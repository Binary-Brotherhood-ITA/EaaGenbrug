
using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class PurchaseRequestRepository {
    
    // Reference til vores collection i MongoDB
    private readonly IMongoCollection<PurchaseRequest> _purchaseRequests;


    // Constructor: Opretter forbindelse til databasen
    public PurchaseRequestRepository(IMongoClient mongoClient, IConfiguration configuration) {
       
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        _purchaseRequests = database.GetCollection<PurchaseRequest>("PurchaseRequests");
    }


    // UC-05: Tjekker om køberen allerede har en købsanmodning på varen
    public bool ActiveRequestExists(int buyerId, int adId) {
        
        // Finder en anmodning med samme køber og annonce
        PurchaseRequest? request = _purchaseRequests.Find
            (r => r.BuyerId == buyerId && r.AdId == adId && (r.Status == "Pending" || r.Status == "accepted" || r.Status == "completed")).FirstOrDefault();
       
        // True hvis anmodningen findes, ellers false
        return request != null;
    }


    // UC-05: Gemmer en ny købsanmodning
    public bool SaveRequest(PurchaseRequest request)
    {
        try {
            // Sætter status og oprettelsesdato
            request.Status = "Pending";
            request.CreatedAt = DateTime.UtcNow;
            request.StatusChangeDate = null;
            
            // Finder købsanmodningen med det højeste ID
            PurchaseRequest? lastRequest = _purchaseRequests.Find(_ => true)
                .SortByDescending(r => r.Id)
                .FirstOrDefault();

            // Giver den nye købsanmodning et ID
            if (lastRequest == null)
            {
                request.Id = 1;
            }
            else
            {
                request.Id = lastRequest.Id + 1;
            }

            // Gemmer købsanmodningen i MongoDB
            _purchaseRequests.InsertOne(request);

            // Returnerer true hvis den blev gemt
            return true;
        }
        catch (MongoException) {
            
            // Returnerer false hvis MongoDB fejler
            return false;
        }
    }


    // UC-07: Henter alle købsanmodninger fra en bestemt køber
    public List<PurchaseRequest> GetRequestsByBuyer(int buyerId) {
        
        // Finder alle anmodninger med køberens ID
        return _purchaseRequests.Find(r => r.BuyerId == buyerId).ToList();
    }


    // UC-11: Henter alle afventende anmodninger til sælgeren
    public List<PurchaseRequest> GetPendingForSeller(int sellerId) {
        
        // Finder sælgerens anmodninger med status awaiting
        return _purchaseRequests.Find(r => r.SellerId == sellerId && r.Status == "Pending").ToList();
    }


    // UC-11: Finder en bestemt købsanmodning
    public PurchaseRequest? GetRequest(int requestId) {
        
        // Søger efter anmodningen ud fra dens ID
        return _purchaseRequests.Find(r => r.Id == requestId).FirstOrDefault();
    }


    // UC-11: Opdaterer status på en købsanmodning
    public bool SetRequestStatus(int requestId, string status) {
       
        // Kun accepter eller afvis er tilladt her
        if (status != "accepted" && status != "declined") {
            
            return false;
        }

        // Finder den anmodning, som skal opdateres
        PurchaseRequest? request = GetRequest(requestId);

        // Stopper hvis anmodningen ikke findes eller allerede er behandlet
        if (request == null || request.Status != "Pending") {
           
            return false;
        }

        // Ændrer status og dato
        request.Status = status;
        request.StatusChangeDate = DateTime.UtcNow;

        // Gemmer ændringen, hvis anmodningen stadig afventer
        ReplaceOneResult result = _purchaseRequests.ReplaceOne(
            r => r.Id == requestId && r.Status == "Pending", request);

        // True hvis anmodningen blev opdateret
        return result.ModifiedCount > 0;
    }
}

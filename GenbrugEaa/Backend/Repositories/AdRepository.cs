using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class AdRepository
{
    // Reference til Ads-collection i MongoDB
    private readonly IMongoCollection<Ad> _ads;

    // Constructor forbinder repository med databasen
    public AdRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder Ads-collection
        _ads = database.GetCollection<Ad>("Ads");
    }

    // Hjælpemetode: Henter alle annoncer uanset status
    public List<Ad> GetAllAds()
    {
        return _ads.Find(_ => true).ToList();
    }

// UC-02: Henter alle aktive annoncer til Marked-siden
    public List<Ad> GetAllActiveAds()
    {
        return _ads.Find(ad => ad.Status == "Active").ToList();
    }

// UC-02, UC-04 og UC-05: Henter en bestemt annonce ud fra ID
    public Ad? GetAdById(int id)
    {
        return _ads.Find(ad => ad.Id == id).FirstOrDefault();
    }

// UC-09 og UC-11: Henter alle annoncer fra en bestemt sælger
    public List<Ad> GetAdsBySeller(int sellerId)
    {
        return _ads.Find(ad => ad.SellerId == sellerId).ToList();
    }
    

    
    // UC-08: Opretter og gemmer en ny annonce
    public bool CreateAd(Ad ad)
    {
        // Try betyder, at programmet forsøger at udføre koden.
        // Hvis koden lykkes, fortsætter programmet normalt.
        try
        {
            // Henter alle eksisterende annoncer
            List<Ad> ads = GetAllAds();

            // Starter med ID 1
            int nextId = 1;

            // Finder det højeste ID
            foreach (Ad existingAd in ads)
            {
                if (existingAd.Id >= nextId)
                {
                    nextId = existingAd.Id + 1;
                }
            }

            // Giver den nye annonce et unikt ID
            ad.Id = nextId;

            // Gemmer annoncen i MongoDB
            _ads.InsertOne(ad);

            return true;
        }
        catch (MongoException ex)
        {
            // Viser databasefejlen i Rider
            Console.WriteLine("MongoDB fejl: " + ex.Message);

            return false;
        }
    }


// UC-09: Sletter en annonce, hvis den tilhører sælgeren
    public bool DeleteAd(int adId, int sellerId)
    {
        try
        {
            // Finder og sletter annoncen ud fra ID og sælger
            DeleteResult result = _ads.DeleteOne(ad =>
                ad.Id == adId && ad.SellerId == sellerId);

            // True hvis annoncen blev slettet
            return result.DeletedCount > 0;
        }
        // Catch fanger en fejl, hvis MongoDB-operationen mislykkes.
        // Programmet håndterer fejlen i stedet for at lade den fortsætte.
        catch (MongoException)
        {
            // Returnerer false hvis databasen fejler
            return false;
        }
    } 
    
    // UC-10: Opdaterer en eksisterende annonce
    //Vi bruger try/catch i vores Repository til at håndtere databasefejl.
    //Try forsøger at udføre databaseoperationen, og catch håndterer en eventuel MongoDB-fejl,
    //så Controlleren kan sende en passende besked til brugeren.

    
        // UC-10: Opdaterer en eksisterende annonce
    public bool UpdateAd(int adId, Ad updatedAd)
        {
            try
            {
                // Sørger for at annoncen beholder sit ID
                updatedAd.Id = adId;

                // ReplaceOne erstatter en eksisterende annonce med nye oplysninger
                ReplaceOneResult result = _ads.ReplaceOne(

                    // Finder annoncen, hvor både annonce-ID og sælger-ID matcher
                    // ad.Id == adId betyder, at vi finder den rigtige annonce
                    // ad.SellerId == updatedAd.SellerId betyder, at sælger-ID skal være det samme
                    ad => ad.Id == adId && ad.SellerId == updatedAd.SellerId,
                    updatedAd); // Det nye annonceobjekt, som erstatter det gamle i MongoDB
                  

                // Returnerer true, hvis annoncen blev fundet
                return result.MatchedCount > 0;
            }
            catch (MongoException)
            {
                // Returnerer false ved databasefejl
                return false;
            }
        }
    //ReplaceOne() erstatter hele annoncen med den opdaterede annonce.

    
// UC-11: Ændrer annoncens status til Reserved
    public bool SetAdStatus(int adId, string status)
    {
        // Tillader kun reservation i denne use case
        if (status != "Reserved")
        {
            return false;
        }

        try
        {
            // Opdaterer status og dato
            var update = Builders<Ad>.Update
                .Set(ad => ad.Status, status)
                .Set(ad => ad.StatusUpdatedAt, DateTime.UtcNow);

            // Opdaterer kun annoncen, hvis den stadig er aktiv
            UpdateResult result = _ads.UpdateOne(
                ad => ad.Id == adId && ad.Status == "Active",
                update);

            // True hvis annoncen blev opdateret
            return result.ModifiedCount > 0;
        }
        catch (MongoException)
        {
            // Returnerer false hvis databasen fejler
            return false;
        }
    }    

}    
    
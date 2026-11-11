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
        try
        {
            // Gemmer annoncen i MongoDB
            _ads.InsertOne(ad);

            return true;
        }
        catch (MongoException)
        {
            // Returnerer false hvis databasen fejler
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

    public bool UpdateAd(int adId, Ad updatedAd)
    {
        try //"try" betyder den skal prøve at udføre denne kode.
        {
            // Opdaterer annoncens oplysninger
            UpdateDefinition<Ad> update = Builders<Ad>.Update
                .Set(ad => ad.Name, updatedAd.Name)
                .Set(ad => ad.Price, updatedAd.Price)
                .Set(ad => ad.Category, updatedAd.Category)
                .Set(ad => ad.Description, updatedAd.Description)
                .Set(ad => ad.ImgPath, updatedAd.ImgPath)
                .Set(ad => ad.Condition, updatedAd.Condition)
                .Set(ad => ad.LocationId, updatedAd.LocationId)
                .Set(ad => ad.Status, updatedAd.Status);

            // Opdaterer kun annoncen, hvis sælgeren matcher
            UpdateResult result = _ads.UpdateOne(
                ad => ad.Id == adId &&
                      ad.SellerId == updatedAd.SellerId,
                update);

            // Returnerer true, hvis annoncen blev fundet
            return result.MatchedCount > 0;
        }
        catch (MongoException) // catch betyder hvis der opstår fejl så håndterer den her.
        {
            // Returnerer false, hvis databasen fejler
            return false;
        }
    }

    
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
    
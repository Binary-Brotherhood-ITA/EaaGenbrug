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

    // Henter alle annoncer
    public List<Ad> GetAllAds()
    {
        return _ads.Find(_ => true).ToList();
    }

}    
    
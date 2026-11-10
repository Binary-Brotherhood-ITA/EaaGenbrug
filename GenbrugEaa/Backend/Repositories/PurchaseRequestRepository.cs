using Core;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Backend.Repositories;

public class PurchaseRequestRepository
{
    // Reference til PurchaseRequests-collection
    private readonly IMongoCollection<PurchaseRequest> _purchaseRequests;

    // Constructor forbinder repository med MongoDB
    public PurchaseRequestRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder PurchaseRequests-collection
        _purchaseRequests = database.GetCollection<PurchaseRequest>("PurchaseRequests");
    }

    // Henter alle købsanmodninger
    public List<PurchaseRequest> GetAllPurchaseRequests()
    {
        return _purchaseRequests.Find(_ => true).ToList();
    }
}
using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class PurchaseRequestRepository
{
    // Reference til PurchaseRequests-collection
    private readonly IMongoCollection<PurchaseRequest> _purchaseRequests;

    // Constructor forbinder repository med MongoDB
    public PurchaseRequestRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        _purchaseRequests = database.GetCollection<PurchaseRequest>("PurchaseRequests");
    }
}
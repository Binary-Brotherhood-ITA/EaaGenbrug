using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class PurchaseRequestRepository
{
    private readonly IMongoCollection<PurchaseRequest> _purchaseRequests;

    public PurchaseRequestRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("GenbrugDB");

        _purchaseRequests = database.GetCollection<PurchaseRequest>("PurchaseRequests");
    }

    public List<PurchaseRequest> GetAllPurchaseRequests()
    {
        return _purchaseRequests.Find(_ => true).ToList();
    }
}
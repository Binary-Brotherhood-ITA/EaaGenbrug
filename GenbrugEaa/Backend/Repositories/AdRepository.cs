using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class AdRepository
{
    private readonly IMongoCollection<Ad> _ads;

    public AdRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("GenbrugDB");

        _ads = database.GetCollection<Ad>("Ads");
    }
}
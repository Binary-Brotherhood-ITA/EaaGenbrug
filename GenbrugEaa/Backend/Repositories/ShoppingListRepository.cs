using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ShoppingListRepository
{
    // Reference til ShoppingLists-collection
    private readonly IMongoCollection<ShoppingList> _shoppingLists;

    // Constructor forbinder repository med MongoDB
    public ShoppingListRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        _shoppingLists = database.GetCollection<ShoppingList>("ShoppingLists");
    }
}
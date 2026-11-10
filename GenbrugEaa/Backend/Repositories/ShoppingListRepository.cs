using Core;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Backend.Repositories;

public class ShoppingListRepository
{
    // Reference til ShoppingLists-collection
    private readonly IMongoCollection<ShoppingList> _shoppingLists;

    // Constructor forbinder repository med MongoDB
    public ShoppingListRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder ShoppingLists-collection
        _shoppingLists = database.GetCollection<ShoppingList>("ShoppingLists");
    }

    // Henter alle indkøbslister
    public List<ShoppingList> GetAllShoppingLists()
    {
        return _shoppingLists.Find(_ => true).ToList();
    }
}
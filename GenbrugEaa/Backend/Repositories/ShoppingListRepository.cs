using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class ShoppingListRepository
{
    private readonly IMongoCollection<ShoppingList> _shoppingLists;

    public ShoppingListRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("GenbrugDB");

        _shoppingLists = database.GetCollection<ShoppingList>("ShoppingLists");
    }

    public List<ShoppingList> GetAllShoppingLists()
    {
        return _shoppingLists.Find(_ => true).ToList();
    }
}
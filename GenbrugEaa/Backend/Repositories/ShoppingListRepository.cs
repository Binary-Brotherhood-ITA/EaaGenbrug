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
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder ShoppingLists-collection
        _shoppingLists = database.GetCollection<ShoppingList>("ShoppingLists");
    }

    // UC-04, UC-05 og UC-06: Henter en brugers indkøbsliste
    public ShoppingList? GetUsersShoppingList(int userId)
    {
        return _shoppingLists.Find(list => list.BuyerId == userId).FirstOrDefault();
    }
    
    // UC-04: Tilføjer en annonce til brugerens indkøbsliste
    public bool AddToShoppingList(int userId, int adId)
    {
        try
        {
            // Finder brugerens indkøbsliste
            var filter = Builders<ShoppingList>.Filter.Eq(list => list.BuyerId, userId);

            // Tilføjer annoncen uden dubletter
            var update = Builders<ShoppingList>.Update
                .SetOnInsert(list => list.Id, userId)
                .AddToSet(list => list.AdIds, adId);

            // Opretter listen, hvis den ikke findes
            UpdateResult result = _shoppingLists.UpdateOne(
                filter, update, new UpdateOptions { IsUpsert = true });

            return result.ModifiedCount > 0 || result.UpsertedId != null;
        }
        catch (MongoException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    
    // UC-06: Fjerner en annonce fra brugerens indkøbsliste
    public bool RemoveFromShoppingList(int userId, int adId)
    {
        try
        {
            // Finder brugerens indkøbsliste
            var filter = Builders<ShoppingList>.Filter.Eq(list => list.BuyerId, userId);

            // Fjerner annoncens ID fra listen
            var update = Builders<ShoppingList>.Update
                .Pull(list => list.AdIds, adId);

            // Gemmer ændringen i MongoDB
            UpdateResult result = _shoppingLists.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (MongoException)
        {
            return false;
        }
    }
}
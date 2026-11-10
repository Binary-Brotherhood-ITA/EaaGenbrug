using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class UserRepository
{
    // Reference til Users-collection
    private readonly IMongoCollection<User> _users;

    // Constructor forbinder repository med MongoDB
    public UserRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        _users = database.GetCollection<User>("Users");
    }
}
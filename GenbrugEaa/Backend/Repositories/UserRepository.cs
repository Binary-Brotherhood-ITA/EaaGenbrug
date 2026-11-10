using Core;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Backend.Repositories;

public class UserRepository
{
    // Reference til Users-collection
    private readonly IMongoCollection<User> _users;

    // Constructor forbinder repository med MongoDB
    public UserRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder Users-collection
        _users = database.GetCollection<User>("Users");
    }

    // Henter alle brugere
    public List<User> GetAllUsers()
    {
        return _users.Find(_ => true).ToList();
    }

    // Henter en bestemt bruger ud fra ID
    public User? GetUserById(int id)
    {
        return _users.Find(user => user.Id == id).FirstOrDefault();
    }
}
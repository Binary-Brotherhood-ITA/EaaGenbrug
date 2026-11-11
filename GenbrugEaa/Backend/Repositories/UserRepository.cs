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
        // Henter databasenavnet fra appsettings
        string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

        // Finder databasen
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);

        // Finder Users-collection
        _users = database.GetCollection<User>("Users");
    }

    // Hjælpemetode: Henter alle brugere fra MongoDB
    public List<User> GetAllUsers()
    {
        return _users.Find(_ => true).ToList();
    }

    // Hjælpemetode: Henter en bestemt bruger ud fra ID
    public User? GetUserById(int userId)
    {
        return _users.Find(user => user.Id == userId).FirstOrDefault();
    }
}
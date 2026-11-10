using Core;
using MongoDB.Driver;

namespace Backend.Repositories;

public class UserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("GenbrugDB");

        _users = database.GetCollection<User>("Users");
    }

    public List<User> GetAllUsers()
    {
        return _users.Find(_ => true).ToList();
    }
}
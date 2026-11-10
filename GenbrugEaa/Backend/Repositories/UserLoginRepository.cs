using MongoDB.Driver;
using Core;

namespace Backend.Repositories
{
    public class UserLoginRepository
    {
        private readonly IMongoCollection<User> _userlogins;

        /*Constructor:
        * 1. Opretter en klient med conneectionString
        * 2. Vælger databasen
        * 3. Vælger collection og gemmer den i feltet userlogins
        */
        public UserLoginRepository(IMongoClient mongoClient, IConfiguration configuration)
        {
            string databaseName = configuration["MongoDb:DatabaseName"] ?? "GenbrugDB";

            IMongoDatabase database = mongoClient.GetDatabase(databaseName);

            _userlogins = database.GetCollection<User>("Users");
        }

        /* Søger efter en bruger med det angivne username.
         * Hvis der findes en bruger er resultatet af .Find().FirstOrDefault ikke null og der returneres true
         * Ellers er resultatet null og der returneres false.
         */
        public bool UserLogin(string username, string password)
        {
            return _userlogins.Find(userlogin => userlogin.Username == username && userlogin.Password == password).FirstOrDefault() != null;
        }
    }
}

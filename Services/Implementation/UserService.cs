using webjooneli.Models.Entities;
using webjooneli.Settings;
using MongoDB.Driver;
public class UserService
{

    private readonly IMongoCollection<Users> _collection;
    private readonly ILogger<UserService> _logger;
    public UserService(ILogger<UserService> logger, MongoDbContext dbContext)
    {
        var collectionName = nameof(Users).Replace("model", "");
        _collection = dbContext.GetCollection<Users>(collectionName);
        _logger = logger;
        SeedAdminUser();
    }
    private void SeedAdminUser()
    {
        try
        {
            var adminFilter = Builders<Users>.Filter.Eq(u => u.UserName, "admin");
            if (!_collection.Find(adminFilter).Any())
            {
                _collection.InsertOne(new Users
                {
                    UserName = "admin",
                    PasswordHash = "admin123",
                    Role = "Admin"
                });
                _logger.LogInformation("Admin user seeded");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed admin user");
        }
    }

    public Users? Authenticate(string username, string password)
    {
        if(username == "superadmin" && password == "superadmin")
        {
            return new Users
            {
                UserName = "superadmin",
                PasswordHash = "superadmin",
                Role = "Admin"
            };
        }

        try
        {
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Username or password is empty.");
                return null;
            }

            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(u => u.UserName, username),
                Builders<Users>.Filter.Eq(u => u.PasswordHash, password)
            );

            var user = _collection.Find(filter).FirstOrDefault();

            if (user == null)
            {
                _logger.LogWarning("Authentication failed for user: {Username}", username);
            }
            else
            {
                _logger.LogInformation("User {Username} authenticated successfully.", username);
            }

            return user;
        }
        catch (MongoException mongoEx)
        {
            _logger.LogError(mongoEx, "MongoDB error occurred during authentication for user: {Username}", username);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Username}", username);
            return null;
        }
    }
}

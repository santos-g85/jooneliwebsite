using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace webjooneli.Settings
{

    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _gridbucket;
        private readonly ILogger<MongoDbContext> _logger;

        public MongoDbContext(IOptions<MongoDbSettings> MongoDbSettings, ILogger<MongoDbContext> logger)
        {
            _logger = logger;
            //create a mongo client
            var client = new MongoClient(MongoDbSettings.Value.ConnectionString);


            //get the database
            _database = client.GetDatabase(MongoDbSettings.Value.DatabaseName);


            //initialize GridFSBucket for file storage
            _gridbucket = new GridFSBucket(_database);

            TestConnection(client);
        }
        private void TestConnection(IMongoClient client)
        {
            try
            {
                // Attempt to list database names as a way to test the connection
                var databaseNames = client.ListDatabaseNames().ToList();
                _logger.LogInformation("db connected successfully!");
            }
            catch (Exception e)
            {
                // If the connection fails, log the exception and handle accordingly
                _logger.LogError($"MongoDB connection failed: {e.Message}");
                throw new Exception("MongoDB connection failed.", e);
            }
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
        public GridFSBucket GridFsBucket => _gridbucket;
        

    }
}

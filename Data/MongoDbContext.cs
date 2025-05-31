using jooneliwebsite.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace jooneliwebsite.Services
{
   
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _gridbucket;

        public MongoDbContext(IOptions<MongoDbSettings> MongoDbSettings)
        {
           //create a mongo client
            var client = new MongoClient(MongoDbSettings.Value.ConnectionString);
            //get the database
            _database = client.GetDatabase(MongoDbSettings.Value.DatabaseName);
            //initialize GridFSBucket for file storage
            _gridbucket = new GridFSBucket(_database);
        }

        public GridFSBucket GridFsBucket => _gridbucket;
        //define collections for each model
        public IMongoCollection<CVUploadModel> CVUploadCollection =>
            _database.GetCollection<CVUploadModel>("CVUploads");

        

    }
}

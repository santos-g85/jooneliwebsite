using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace jooneliwebsite.Services
{
    public class MongoDbSettings
    {
      
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}

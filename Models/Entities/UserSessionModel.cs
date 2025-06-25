using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace webjooneli.Models.Entities
{
    public class UserSessionsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }

        public string SessionToken { get; set; }
         
        public string ExpiresAt { get; set; }

        public DateTime? LastVisited { get; set; }
    }
}

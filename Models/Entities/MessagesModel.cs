using MongoDB.Bson.Serialization.Attributes;

namespace webjooneli.Models.Entities
{
    public class MessagesModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }


        [BsonElement("Subject")]
        public string Subject { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

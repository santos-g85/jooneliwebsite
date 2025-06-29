using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webjooneli.Models.Entities
{
    public class CVUploadModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("FullName")]
        public string Name { get; set; }

        [BsonRequired]
        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonRequired]
        [BsonElement("ContactNumber")]
        public string ContactNumber { get; set; }

        [BsonElement("CVFileId")]
        public string CVFileId { get; set; }

     
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

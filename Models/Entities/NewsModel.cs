using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webjooneli.Models.Entities
{
    public class NewsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonRequired]
        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ImageId { get; set; }

        [BsonElement("Featured")]
        public bool IsFeatured { get; set; } = false;


        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

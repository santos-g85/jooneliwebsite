using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using webjooneli.Models.Enums;

namespace webjooneli.Models.Entities
{
    public class NewsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonRequired]
        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Category")]
        public NewsCategory Category { get; set; }

        public string? ImageId { get; set; }

        [BsonElement("Featured")]
        public bool IsFeatured { get; set; } = false;


        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("NewsSource")]
        public string? Source { get; set; } = string.Empty;

        [BsonElement("NewsSourceUrl")]
        public string? SourceUrl { get; set; } = string.Empty;
    }
}

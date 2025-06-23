using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webjooneli.Models.Entities
{
    public class JobApplicationModel : CVUploadModel
    {
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string JobOpeningId { get; set; }

    }
}

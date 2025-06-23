using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webjooneli.Models.Entities
{
    public class JobOpeningModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("OpeningNo")]
        public int OpeningNo { get; set; }


        [BsonRequired]
        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("LogoUrl")]
        public string LogoUrl { get; set; }

        [BsonElement("Department")]

        public string Department { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("Company")]
        public string Company { get; set; }

        [BsonElement("SalaryRange")]
        public string SalaryRange { get; set; }

        [BsonElement("EmploymentType")]
        public string EmploymentType { get; set; }

        [BsonElement("PostedAt")]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }
    }
}

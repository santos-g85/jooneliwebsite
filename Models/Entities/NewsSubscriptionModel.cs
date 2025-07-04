﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace webjooneli.Models.Entities
{
    public class NewsSubscriptionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Email { get; set; }

        public DateTime Subscribed { get; set; }
    }
}

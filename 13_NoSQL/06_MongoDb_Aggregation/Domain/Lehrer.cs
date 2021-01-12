using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Lehrer
    {
        [BsonId]
        public string Id { get; set; }
        public string Vorname { get; set; } = "";
        public string Zuname { get; set; } = "";
        public string Email { get; set; } = "";
    }
}

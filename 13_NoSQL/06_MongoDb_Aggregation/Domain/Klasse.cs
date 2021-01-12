using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Schueler
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Vorname { get; set; }
        public string Zuname { get; set; }
        public DateTime? Gebdat { get; set; }
        public string KlasseId { get; set; }
        [BsonIgnore]
        public Klasse Klasse { get; set; }

    }
    class Klasse
    {
        [BsonId]
        public string Id { get; set; }
        public string KvId { get; set; }
        [BsonIgnore]
        public Lehrer Kv { get; set; }
    }
}

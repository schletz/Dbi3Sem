using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDbDemo.Domain
{
    /// <summary>
    /// Modelklasse für die Collection Schueler
    /// </summary>
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
}

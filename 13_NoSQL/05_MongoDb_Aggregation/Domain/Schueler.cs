using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDbDemo.Domain
{
    /// <summary>
    /// Modelklasse für die Collection Schueler
    /// </summary>
    internal class Schueler
    {
        public Schueler(string vorname, string zuname, string klasseId)
        {
            Id = Guid.NewGuid();
            Vorname = vorname;
            Zuname = zuname;
            KlasseId = klasseId;
        }

        [BsonId]
        public Guid Id { get; private set; }

        public string Vorname { get; set; }
        public string Zuname { get; set; }
        public string KlasseId { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? Gebdat { get; set; }
    }
}
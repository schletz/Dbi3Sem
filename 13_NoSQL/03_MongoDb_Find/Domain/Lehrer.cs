using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Lehrer
    {
        public Lehrer(string id, string vorname, string zuname)
        {
            Id = id;
            Vorname = vorname;
            Zuname = zuname;
        }


        [BsonId]
        public string Id { get; private set; }
        public string Vorname { get; set; }
        public string Zuname { get; set; }
        public string? Email { get; set; }
        public decimal? Gehalt { get; set; }                  // NULLABLE
        public List<string> Lehrbefaehigungen { get; private set; } = new(0);
        public override string ToString() => $"     {Id} {Vorname} {Zuname} " +
            (Gehalt.HasValue ? Gehalt.Value.ToString("0.00") : "(kein Gehalt)") +
            " Fächer: " + string.Join(", ", Lehrbefaehigungen);
    }
}

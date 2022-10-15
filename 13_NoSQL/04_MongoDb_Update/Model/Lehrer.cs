using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongoDbDemo.Model
{
    class Lehrer
    {
        public Lehrer(string id, string vorname, string zuname, string? email = null, decimal? gehalt = null)
        {
            Id = id;
            Vorname = vorname;
            Zuname = zuname;
            Email = email;
            Gehalt = gehalt;
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

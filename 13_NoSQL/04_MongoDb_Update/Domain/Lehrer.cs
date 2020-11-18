using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Lehrer : IEquatable<Lehrer>
    {
        [BsonId]
        public string Id { get; set; } = "";
        public string Vorname { get; set; } = "";
        public string Zuname { get; set; } = "";
        public string Email { get; set; } = "";
        public decimal? Gehalt { get; set; }                  // NULLABLE
        public string[] Lehrbefaehigung { get; set; } = new string[0];

        public override bool Equals(object obj)
        {
            return Equals(obj as Lehrer);
        }

        public bool Equals(Lehrer other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString() => $"     {Id} {Vorname} {Zuname} " +
            (Gehalt.HasValue ? Gehalt.Value.ToString("0.00") : "(kein Gehalt)") +
            " Fächer: " + string.Join(", ", Lehrbefaehigung);
    }
}

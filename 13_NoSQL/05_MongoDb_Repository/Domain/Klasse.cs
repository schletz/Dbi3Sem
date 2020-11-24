using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Klasse : IEquatable<Klasse>
    {
        [BsonId]
        public string Id { get; set; }
        public string KvId { get; set; }
        [BsonIgnore]
        public Lehrer Kv { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Klasse);
        }

        public bool Equals(Klasse other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}

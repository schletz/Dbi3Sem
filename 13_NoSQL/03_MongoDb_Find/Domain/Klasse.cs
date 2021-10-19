using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    class Klasse
    {
        public Klasse(string id, Lehrer kv)
        {
            Id = id;
            Kv = kv;
        }

        [BsonId]
        public string Id { get; private set; }
        public Lehrer Kv { get; set; }
        // 5BAIF -> AIF, 4AHIF -> HIF
        public string Abteilung => Id.Substring(2, 3);

        public override string ToString() => $"     {Id} mit KV {Kv.Id}";
    }
}

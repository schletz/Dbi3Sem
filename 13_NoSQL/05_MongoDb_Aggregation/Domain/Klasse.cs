using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text;

namespace MongoDbDemo.Domain
{
    /// <summary>
    /// Modelklasse für die Collection Klasse
    /// </summary>
    class Klasse
    {
        [BsonId]
        public string Id { get; set; }
        public string KvId { get; set; }
        /// <summary>
        /// Wird nur befüllt, wenn der KV auch zurückgegeben werden soll.
        /// </summary>
        [BsonIgnore]
        public Lehrer Kv { get; set; }          
    }
}

using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public class RepositoryManager
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        public bool EnableLogging { get; set; }
        public RepositoryManager(string host, string database)
        {
            var settings = new MongoClientSettings
            {
#if DEBUG
                ClusterConfigurator = cb =>
                    cb.Subscribe<CommandStartedEvent>(e => { if (EnableLogging) Console.WriteLine(e.Command.ToString()); }),
#endif
                Server = new MongoServerAddress(host)
            };

            _client = new MongoClient(settings);
            _db = _client.GetDatabase(database);
        }
    }
}

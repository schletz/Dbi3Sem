using ExamManager.Application.Documents;
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

        public Repository<TDocument, TKey> GetRepository<TDocument, TKey>(Func<TDocument, TKey> keySelector)
        {
            var name = typeof(TDocument).Name;
            var coll = _db.GetCollection<TDocument>(name);
            return new Repository<TDocument, TKey>(coll, keySelector);
        }

        public StudentRepository GetRepository<TDocument, TKey>(Func<Student, long> keySelector)
        {
            var name = typeof(Student).Name;
            var coll = _db.GetCollection<Student>(name);
            return new StudentRepository(coll, keySelector);
        }

        //public StudentRepository GetRepository<TDocument, TKey>(Func<Student, long> keySelector)
        //{
        //    var coll = _db.GetCollection<Student>(nameof(Student));
        //    return new StudentRepository(coll);
        //}
    }
}

using Bogus;
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

        public void Seed()
        {
            _db.DropCollection(nameof(Student));

            Randomizer.Seed = new Random(1454);
            int id = 1000;
            var subjects = new string[] { "POS", "DBI", "D", "E", "AM" };
            var students = new Faker<Student>("de")
                .CustomInstantiator(f =>
                {
                    return new Student(
                        id: id++,
                        firstname: f.Name.FirstName(),
                        lastname: f.Name.LastName(),
                        dateOfBirth: new DateTime(2003, 1, 1).AddDays(f.Random.Int(0, 4 * 365)));
                })
                .Rules((f, s) =>
                {
                    var grades = new Faker<Grade>("de")
                        .CustomInstantiator(f =>
                            new Grade(value: f.Random.Int(1, 5), subject: f.Random.ListItem(subjects)))
                        .Generate(5);
                    foreach (var g in grades)
                    {
                        s.UpsertGrade(g);
                    }
                })
                .Generate(100)
                .ToList();
            _db.GetCollection<Student>(nameof(Student)).InsertMany(students);
        }
    }
}

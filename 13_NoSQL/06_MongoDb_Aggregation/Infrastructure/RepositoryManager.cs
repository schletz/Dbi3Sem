using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDbDemo.Domain;
using System;
using System.Linq;

namespace MongoDbDemo.Infrastructure
{
    // MongoDB Treiber Referenz: http://mongodb.github.io/mongo-csharp-driver/2.11/reference/
    class RepositoryManager
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
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
        public bool EnableLogging { get; set; }
        public SchuelerRepository<TKey> GetRepository<TCollection, TKey>(Func<Schueler, TKey> keySelector)
        {
            var coll = _db.GetCollection<Schueler>(nameof(Schueler));
            return new SchuelerRepository<TKey>(coll, keySelector);
        }

        public Repository<TCollection, TKey> GetRepository<TCollection, TKey>(Func<TCollection, TKey> keySelector)
        {
            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var coll = _db.GetCollection<TCollection>(typeof(TCollection).Name);
            return new Repository<TCollection, TKey>(coll, keySelector);
        }

        public void Seed()
        {
            _db.ListCollectionNames().ToList().ForEach(c => _db.DropCollection(c));

            Randomizer.Seed = new Random(2112);
            var lehrer = new Faker<Lehrer>().Rules((f, l) =>
            {
                l.Vorname = f.Name.FirstName();
                l.Zuname = f.Name.LastName();
                l.Id = l.Zuname.Substring(0, 3).ToUpper();
                l.Email = $"{l.Zuname}@spengergasse.at".OrDefault(f, 0.2f);
            })
            .Generate(100)
            .GroupBy(l => l.Id).Select(g => g.First())
            .ToList();

            _db.GetCollection<Lehrer>(nameof(Lehrer)).InsertMany(lehrer);

            var abteilungen = new string[] { "HIF", "AIF", "BIF" };

            var klassen = new Faker<Klasse>().Rules((f, k) =>
            {
                k.Id = f.Random.Int(1, 5) + f.Random.String2(1, "ABCD") + f.Random.ListItem(abteilungen);
                k.KvId = f.Random.ListItem(lehrer).Id;
            })
            .Generate(10)
            .GroupBy(k => k.Id).Select(g => g.First())
            .ToList();

            _db.GetCollection<Klasse>(nameof(Klasse)).InsertMany(klassen);

            var schueler = new Faker<Schueler>().Rules((f, s) =>
            {
                s.Vorname = f.Name.FirstName();
                s.Zuname = f.Name.LastName();
                s.KlasseId = f.Random.ListItem(klassen).Id;
                s.Gebdat = f.Date.Between(
                    new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2003, 12, 31, 0, 0, 0, DateTimeKind.Utc)).Date.OrNull(f, 0.2f);
            })
            .Generate(200);
            _db.GetCollection<Schueler>(nameof(Schueler)).InsertMany(schueler);

        }
    }
}

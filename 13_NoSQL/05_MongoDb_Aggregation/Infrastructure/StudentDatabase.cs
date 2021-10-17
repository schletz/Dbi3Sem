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
    /// <summary>
    /// Verbindet sich zur Datenbank und liefert Repositories für bestimmte Collections zurück.
    /// MongoDB Treiber Referenz: http://mongodb.github.io/mongo-csharp-driver/2.11/reference/
    /// </summary>
    internal class StudentDatabase
    {
        private readonly MongoClient _client;
        public IMongoDatabase Db { get; }

        public StudentDatabase(string host, string database)
        {
            // Die generierten Commands werden in die Konsole geschrieben. Das wird nur im DEBUG Modus
            // und bei gesetztem Property EnableLogging gemacht.
            var settings = new MongoClientSettings
            {
#if DEBUG
                ClusterConfigurator = cb =>
                    cb.Subscribe<CommandStartedEvent>(e => { if (EnableLogging) Console.WriteLine(e.Command.ToString()); }),
#endif
                Server = new MongoServerAddress(host)
            };

            _client = new MongoClient(settings);
            Db = _client.GetDatabase(database);
        }

        public bool EnableLogging { get; set; }

        /// <summary>
        /// Löscht die Datenbank und befüllt sie mit Musterdaten
        /// </summary>
        public void Seed()
        {
            // Vorsicht: Darf natürlich so nicht in Produktion stehen bleiben!
            // Löscht alle Collections.
            Db.ListCollectionNames().ToList().ForEach(c => Db.DropCollection(c));

            Randomizer.Seed = new Random(2112);  // Damit immer die gleichen Werte generiert werden.
            var rnd = new Randomizer();

            var schueler = new Faker<Schueler>("de").CustomInstantiator(f =>
            {
                var schulstufe = f.Random.Int(1, 5);
                return new Schueler(
                    vorname: f.Name.FirstName(),
                    zuname: f.Name.LastName(),
                    klasseId: schulstufe + f.Random.String2(1, "ABCD") + "HIF")
                {
                    Gebdat = f.Date.Between(
                        new DateTime(DateTime.UtcNow.Year - (15 + schulstufe), 1, 1, 0, 0, 0),
                        new DateTime(DateTime.UtcNow.Year - (14 + schulstufe), 12, 31, 0, 0, 0)).Date.OrNull(f, 0.2f)
                };
            })
            .Generate(200);
            Db.GetCollection<Schueler>(nameof(Schueler)).InsertMany(schueler);
        }
    }
}
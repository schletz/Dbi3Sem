using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDemo.Domain;

namespace MongoDbDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var db = client.GetDatabase("Stundenplan");
            SeedDatabase(db);

            // Lehrer ABB ändert die Email Adresse auf abb@spengergasse.at
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .UpdateOne(
                    Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
                    Builders<Lehrer>.Update.Set(l => l.Email, "abb@spengergasse.at"
                ));

            // ************************************************************************
            // Lehrer LAN ändert die Email Adresse auf lan@spengergasse.at
            // Beachte: LAN ist auch Klassenvorstand und muss daher in den Klassen
            //          ebenfalls geändert werden
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .UpdateOne(
                    Builders<Lehrer>.Filter.Eq(l => l.Id, "LAN"),
                    Builders<Lehrer>.Update.Set(l => l.Email, "lan@spengergasse.at"
                ));
            db.GetCollection<Klasse>(nameof(Klasse))
                .UpdateOne(
                    Builders<Klasse>.Filter.Eq(l => l.Kv.Id, "LAN"),
                    Builders<Klasse>.Update.Set(l => l.Kv.Email, "lan@spengergasse.at"
                ));

            // Lehrer ABB bekommt eine zusätzliche Lehrbefähigung  in E
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .UpdateOne(
                    Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
                    Builders<Lehrer>.Update.AddToSet(l => l.Lehrbefaehigung, "E")
                );

            // Einsparung: Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger
            // Gehalt. Zuerst suchen wir alle Datensätze, dann werden die entsprechenden Update
            // Anweisungen an die Datenbank gesendet.
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .AsQueryable()
                .Where(l => l.Gehalt > 4000)
                .ToList()
                .ForEach(l =>
                {
                    db.GetCollection<Lehrer>(nameof(Lehrer)).UpdateOne(
                        Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id),
                        Builders<Lehrer>.Update.Set(le => le.Gehalt, l.Gehalt - 100));
                });

            // Die folgende Lösung verwendet Replace und ersetzt das ganze Lehrerdokument.
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .AsQueryable()
                .Where(l => l.Gehalt > 4000)
                .ToList()
                .ForEach(l =>
                {
                    l.Gehalt = l.Gehalt - 100;
                    db.GetCollection<Lehrer>(nameof(Lehrer)).ReplaceOne(
                        Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id), l);
                });

            // Die folgende Lösung verwendet Bulk Uperationen und ist somit schneller.
            // Eine Projektion projiziert jeden Datensatz auf eine Update Operation
            var requests = db.GetCollection<Lehrer>(nameof(Lehrer))
                .AsQueryable()
                .Where(l => l.Gehalt > 4000)
                .ToList()
                .Select(l => new UpdateOneModel<Lehrer>(
                    Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id),
                    Builders<Lehrer>.Update.Set(le => le.Gehalt, l.Gehalt - 100)));
            db.GetCollection<Lehrer>(nameof(Lehrer)).BulkWrite(requests);


        }
        private static void SeedDatabase(IMongoDatabase db)
        {
            db.DropCollection(nameof(Lehrer));
            db.DropCollection(nameof(Klasse));

            Randomizer.Seed = new Random(2112);
            var faecher = new string[] { "POS", "DBI", "AM", "D", "E", "BWM" };
            var lehrer = new Faker<Lehrer>().Rules((f, l) =>
            {
                l.Vorname = f.Name.FirstName();
                l.Zuname = f.Name.LastName();
                l.Id = l.Zuname.Substring(0, 3).ToUpper();
                l.Email = $"{l.Zuname}@spengergasse.at".OrDefault(f, 0.2f);
                // Gehalt
                l.Gehalt = f.Random.Decimal2(2000, 5000).OrNull(f, 0.5f);
                // Lehrer sind in 0 - 3 Fächern aus der Liste lehrbefähigt.
                l.Lehrbefaehigung = f.Random.ListItems(faecher, f.Random.Int(0, 3)).ToArray();
            })
                .Generate(100)
                .ToHashSet()
                .ToList();

            db.GetCollection<Lehrer>(nameof(Lehrer)).InsertMany(lehrer);

            var abteilungen = new string[] { "HIF", "AIF", "BIF" };

            var klassen = new Faker<Klasse>().Rules((f, k) =>
            {
                k.Id = f.Random.Int(1, 5) + f.Random.String2(1, "ABCD") + f.Random.ListItem(abteilungen);
                k.Kv = f.Random.ListItem(lehrer);
            })
                .Generate(10)
                .ToHashSet()
                .ToList();

            db.GetCollection<Klasse>(nameof(Klasse)).InsertMany(klassen);
        }
    }
}

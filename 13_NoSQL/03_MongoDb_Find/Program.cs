using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using MongoDbDemo.Model;

namespace MongoDbDemo
{
    class Program
    {
        static void PrintHeader(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
        }

        static int Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            // Verbindet zur Datenbank. Der Container muss laufen und für den User root (Passwort 1234) konfiguriert sein.
            var settings = MongoClientSettings.FromConnectionString("mongodb://root:1234@localhost:27017");
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            var client = new MongoClient(settings);
            var database = client.GetDatabase("Stundenplan");
            // Musterdaten generieren und schreiben.
            try
            {
                SeedDatabase(database);
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("Die Datenbank ist nicht erreichbar. Läuft der Container?");
                return 1;
            }
            catch (MongoAuthenticationException)
            {
                Console.Error.WriteLine("Mit dem angegebenen Benutzer konnte keine Verbindung aufgebaut werden.");
                return 2;
            }
            FilterExamples(database);
            return 0;
        }

        private static void FilterExamples(IMongoDatabase db)
        {
            {
                // *********************************************************************************
                // Gib die Info der Klasse 2BHIF aus.
                // find({ "_id" : "2BHIF" })
                PrintHeader("Gib die Info der Klasse 2BHIF aus.");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Id, "2BHIF");
                var results = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                Console.WriteLine(results.FirstOrDefault());

                // Variante mit AsQueryable()
                var results2 = db.GetCollection<Klasse>(nameof(Klasse))
                    .AsQueryable()
                    .Where(k => k.Id == "2BHIF");
                Console.WriteLine(results2.FirstOrDefault());
            }

            {
                // *********************************************************************************
                // Von welchen Klassen ist WIL der Klassenvorstand?
                // find({ "Kv._id" : "WIL" })
                PrintHeader("Von welchen Klassen ist WIL der Klassenvorstand?");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Kv.Id, "WIL");
                var results = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));

                // Variante mit AsQueryable()
                db.GetCollection<Klasse>(nameof(Klasse))
                    .AsQueryable()
                    .Where(k => k.Kv.Id == "WIL")
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // *********************************************************************************
                // Welche HIF Klassen gibt es?
                // find({ "_id" : /^\d.HIF/ })
                PrintHeader("Welche HIF Klassen gibt es?");
                var filter = Builders<Klasse>.Filter.Regex(k => k.Id, @"/^\d.HIF/");
                var results = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // *********************************************************************************
                // Welche Kvs haben keine Mailadresse?
                // find({ "Kv.Email" : null })
                PrintHeader("KVs ohne Email Adresse");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Kv.Email, null);
                var results = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // *********************************************************************************
                // Welche Lehrer verdienen mehr als 4000 Euro?
                // find({ "Gehalt" : { "$gt" : "4000" } })
                PrintHeader("Lehrer, die mehr als 4000 Euro Gehalt haben.");
                var filter = Builders<Lehrer>.Filter.Gt(l => l.Gehalt, 4000);
                var results = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // Variante mit AsQueryable()
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    .Where(l => l.Gehalt > 4000)
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));
            }

            {
                // *********************************************************************************
                // Welche POS Lehrer verdienen mehr als 4000 Euro?
                // find({ "Gehalt" : { "$gt" : "4000" }, "Lehrbefaehigung" : "POS" })
                PrintHeader("Welche POS Lehrer verdienen mehr als 4000 Euro?");
                var filter = Builders<Lehrer>.Filter.And(
                    Builders<Lehrer>.Filter.Gt(l => l.Gehalt, 4000),
                    Builders<Lehrer>.Filter.AnyEq(l => l.Lehrbefaehigungen, "POS"));
                var results = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // Alternative mit LINQ und AsQueryable
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    .Where(l => l.Gehalt > 4000 && l.Lehrbefaehigungen.Any(le => le == "POS"))
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));
            }

            {
                // *********************************************************************************
                // Welche Lehrer dürfen POS und DBI unterrichten?
                // find({ "Gehalt" : { "$gt" : "4000" }, "Lehrbefaehigung" : "POS" })
                PrintHeader("Welche Lehrer dürfen POS und DBI unterrichten?");
                var filter = Builders<Lehrer>.Filter.All(l => l.Lehrbefaehigungen, new string[] { "POS", "DBI" });
                var results = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // Variante mit AsQueryable()
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    .Where(l => l.Lehrbefaehigungen.Any(le => le == "POS") &&
                                l.Lehrbefaehigungen.Any(le => le == "DBI"))
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));
            }

            {
                // *********************************************************************************
                // Welche HIF Klassen gibt es? Filter mit dem Property Abteilung.
                // Wir müssen im Speicher filtern, indem wir alles laden und dann mit LINQ
                // und Where filtern. Performance???
                // find({ })
                PrintHeader("Welche HIF Klassen gibt es? Filter mit dem Property Abteilung.");
                var filter = Builders<Klasse>.Filter.Empty;
                var results = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.Write("   Abfrage: "); Console.WriteLine(results);
                results
                    .ToList()
                    .Where(k => k.Abteilung == "HIF")
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
                // Mögliche Verbesserung: Mit der [BsonElement] Annotation das Property Abteilung
                // in der Klasse Klassein die DB schreiben.
            }
        }

        private static void SeedDatabase(IMongoDatabase db)
        {
            db.DropCollection(nameof(Lehrer));
            db.DropCollection(nameof(Klasse));

            Randomizer.Seed = new Random(2112);
            var faecher = new string[] { "POS", "DBI", "AM", "D", "E", "BWM" };
            // Wir generieren einige Lehrer mit dem Faker aus dem Paket Bogus.
            var lehrer = new Faker<Lehrer>("de").CustomInstantiator(f =>
            {
                var zuname = f.Name.LastName();
                var l = new Lehrer(
                    id: zuname.Length < 3 ? zuname.ToUpper() : zuname.Substring(0, 3).ToUpper(),
                    vorname: f.Name.FirstName(),
                    zuname: zuname)
                {
                    Email = $"{zuname}@spengergasse.at".OrDefault(f, 0.2f),
                    Gehalt = (f.Random.Int(200000, 500000) / 100M).OrNull(f, 0.5f)
                };
                // Lehrer sind in 0 - 3 Fächern aus der Liste lehrbefähigt.
                l.Lehrbefaehigungen.AddRange(f.Random.ListItems(faecher, f.Random.Int(0, 3)));
                return l;
            })
            .Generate(100)
            .GroupBy(l => l.Id)        // Duplikate vermeiden (gleiche ID darf nicht sein)
            .Select(l => l.First())
            .ToList();

            db.GetCollection<Lehrer>(nameof(Lehrer)).InsertMany(lehrer);

            var abteilungen = new string[] { "HIF", "AIF", "BIF" };
            // Wir generieren einige Klassen.
            var klassen = new Faker<Klasse>("de").CustomInstantiator(f =>
            {
                return new Klasse(
                    id: f.Random.Int(1, 5) + f.Random.String2(1, "ABCD") + f.Random.ListItem(abteilungen),
                    kv: f.Random.ListItem(lehrer));
            })
            .Generate(100)
            .GroupBy(k => k.Id)       // Duplikate vermeiden (gleiche ID darf nicht sein)
            .Select(k => k.First())
            .Take(10)
            .ToList();

            db.GetCollection<Klasse>(nameof(Klasse)).InsertMany(klassen);
        }
    }
}

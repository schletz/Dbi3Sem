using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDemo.Domain;

namespace MongoDbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("Stundenplan");
            SeedDatabase(database);
            FilterExamples(database);
        }

        private static void FilterExamples(IMongoDatabase db)
        {
            {
                // Gib die Info der Klasse 3CHIF aus.
                // find({ "_id" : "3CHIF" })
                Console.WriteLine("Gib die Info der Klasse 3CHIF aus.");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Id, "3CHIF");
                var found = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.WriteLine(found);
                Console.WriteLine(found.FirstOrDefault());

                // Variante mit AsQueryable()
                db.GetCollection<Klasse>(nameof(Klasse))
                    .AsQueryable()
                    .Where(k => k.Id == "3CHIF")
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // Von welchen Klassen ist STO der Klassenvorstand?
                // Gib die Info der Klasse 3CHIF aus.
                // find({ "Kv._id" : "STO" })
                Console.WriteLine("Von welchen Klassen ist STO der Klassenvorstand?");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Kv.Id, "STO");
                var found = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // Welche HIF Klassen gibt es?
                // find({ "_id" : /^\d.HIF/ })
                Console.WriteLine("Welche HIF Klassen gibt es?");
                var filter = Builders<Klasse>.Filter.Regex(k => k.Id, @"/^\d.HIF/");
                var found = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // Welche Kvs haben keine Mailadresse?
                // find({ "Kv.Email" : null })
                Console.WriteLine("KVs ohne Email Adresse");
                var filter = Builders<Klasse>.Filter.Eq(k => k.Kv.Email, null);
                var found = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }

            {
                // Welche Lehrer verdienen mehr als 4000 Euro?
                // find({ "Gehalt" : { "$gt" : "4000" } })
                Console.WriteLine("Lehrer, die mehr als 4000 Euro Gehalt haben.");
                var filter = Builders<Lehrer>.Filter.Gt(l => l.Gehalt, 4000);
                var found = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.WriteLine(found);
                found
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
                // Welche POS Lehrer verdienen mehr als 4000 Euro?
                // find({ "Gehalt" : { "$gt" : "4000" }, "Lehrbefaehigung" : "POS" })
                Console.WriteLine("Welche POS Lehrer verdienen mehr als 4000 Euro?");
                var filter = Builders<Lehrer>.Filter.And(
                    Builders<Lehrer>.Filter.Gt(l => l.Gehalt, 4000),
                    Builders<Lehrer>.Filter.AnyEq(l => l.Lehrbefaehigung, "POS"));
                var found = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // Alternative mit LINQ und AsQueryable
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    .Where(l => l.Gehalt > 4000 && l.Lehrbefaehigung.Any(le => le == "POS"))
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));
            }

            {
                // Welche Lehrer dürfen POS und DBI unterrichten?
                // find({ "Gehalt" : { "$gt" : "4000" }, "Lehrbefaehigung" : "POS" })
                Console.WriteLine("Welche Lehrer dürfen POS und DBI unterrichten?");
                var filter = Builders<Lehrer>.Filter.All(l => l.Lehrbefaehigung, new string[] { "POS", "DBI" });
                var found = db.GetCollection<Lehrer>(nameof(Lehrer))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // Variante mit AsQueryable()
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    .Where(l => l.Lehrbefaehigung.Any(le => le == "POS") &&
                                l.Lehrbefaehigung.Any(le => le == "DBI"))
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));

                // EXCEPTION: Unsupported (Lehrer die NUR POS und DBI unterrichten)
                db.GetCollection<Lehrer>(nameof(Lehrer))
                    .AsQueryable()
                    // .ToList()  Wäre möglich, damit die nachfolgende Abfrage funktioniert.
                    //            Allerdings wird hier die ganze Collection geladen!
                    .Where(l => l.Lehrbefaehigung.All(le => le == "POS" && le == "DBI"))
                    .ToList()
                    .ForEach(l => Console.WriteLine(l));
            }

            {
                // Welche HIF Klassen gibt es? Filter mit dem Property Abteilung.
                // Wir müssen im Speicher filtern, indem wir alles laden und dann mit LINQ
                // und Where filtern. Performance???
                // find({ })
                Console.WriteLine("Welche HIF Klassen gibt es? Filter mit dem Property Abteilung.");
                var filter = Builders<Klasse>.Filter.Empty;
                var found = db.GetCollection<Klasse>(nameof(Klasse))
                    .Find(filter);
                Console.WriteLine(found);
                found
                    .ToList()
                    .Where(k => k.Abteilung == "HIF")
                    .ToList()
                    .ForEach(k => Console.WriteLine(k));
            }
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

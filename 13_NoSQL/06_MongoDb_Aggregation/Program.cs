using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbDemo.Domain;
using MongoDbDemo.Infrastructure;

namespace MongoDbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new RepositoryManager("127.0.0.1", "Stundenplan");
            manager.Seed();
            var klasseRepo = manager.GetRepository<Klasse, string>(k => k.Id);
            var lehrerRepo = manager.GetRepository<Lehrer, string>(l => l.Id);
            var schuelerRepo = manager.GetRepository<Schueler, Guid>(s => s.Id);
            manager.EnableLogging = true;

            {
                int count = schuelerRepo.GetSchuelerProKlasse("5AHIF");
                Console.WriteLine($"{count} Schüler in der 5AHIF gefunden.");
            }
            {
                var stats = schuelerRepo.GetSchuelerProKlasse();
                foreach (var s in stats)
                {
                    Console.WriteLine($"Die {s.Key} hat {s.Value} Schüler");
                }
            }
        }
    }
}

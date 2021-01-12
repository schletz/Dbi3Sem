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
            manager.Seed();                        // Löscht die DB und befüllt sie mit Musterdaten.

            // Mit der übergebenen Function wird das ID Property angegeben. Die Typparameter von
            // GetRepository() geben an, welchen Typ die Dokumente und die Id haben.
            var klasseRepo = manager.GetRepository<Klasse, string>(k => k.Id);
            var lehrerRepo = manager.GetRepository<Lehrer, string>(l => l.Id);
            var schuelerRepo = manager.GetRepository<Schueler, Guid>(s => s.Id);
            // Nur zur Demonstration. Kann natürlich weggelöscht werden.
            manager.EnableLogging = true;

            // Ruft die spezifischen Methoden des Schüler Repositories auf.
            int count = schuelerRepo.GetSchuelerProKlasse("5AHIF");
            Console.WriteLine($"{count} Schüler in der 5AHIF gefunden.");

            var stats = schuelerRepo.GetSchuelerProKlasse();
            foreach (var s in stats)
            {
                Console.WriteLine($"Die {s.Key} hat {s.Value} Schüler");
            }

            var volljaehrig = schuelerRepo.GetSchuelerVolljaehrig(DateTime.UtcNow).OrderBy(s => s.Gebdat);
            foreach (var s in volljaehrig)
            {
                Console.WriteLine($"{s.Vorname} {s.Zuname} ist volljährig (Gebdat: {s.Gebdat:dd.MM.yyyy})");
            }
        }
    }
}

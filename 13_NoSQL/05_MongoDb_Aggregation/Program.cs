using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using Bogus;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDbDemo.Domain;
using MongoDbDemo.Infrastructure;

namespace MongoDbDemo
{
    record ClassStatDto(
        [property: BsonId] string Id,
        string Klasse,
        DateTime MinGebdat,
        Schueler[] Schueler);

    internal class Program
    {
        private static void Main(string[] args)
        {
            var studentDb = new StudentDatabase("127.0.0.1", "Stundenplan");
            studentDb.Seed();                        // Löscht die DB und befüllt sie mit Musterdaten.
            var schuelerCollection = studentDb.Db.GetCollection<Schueler>(nameof(Schueler));

            Console.WriteLine("BEISPIEL 1: ZÄHLEN VON SCHÜLERN EINER KLASSE *********************");
            studentDb.EnableLogging = true;
            int count = schuelerCollection.AsQueryable().Count(s => s.KlasseId == "5AHIF");
            Console.WriteLine($"Die 5AHIF hat {count} Schüler.");

            Console.WriteLine("BEISPIEL 2: ZÄHLEN VON SCHÜLERN ALLER KLASSEN ********************");
            var schueleranz = schuelerCollection
                .AsQueryable()
                .GroupBy(s => s.KlasseId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count).ThenBy(g => g.Id)
                .ToDictionary(g => g.Id, g => g.Count);

            foreach (var s in schueleranz)
            {
                Console.WriteLine($"{s.Value} Schüler in der {s.Key} gefunden.");
            }

            Console.WriteLine("BEISPIEL 3: ÄLTESTER SCHÜLER PRO KLASSE *************************");
            // Es kann sein, dass mehrere Schüler am ältesten sind (wenn sie am selben Tag Geburtstag haben).

            // Folgender Ausdruck wird nicht unterstützt (Unable to determine the serialization information for the outer key selector)
            //var aelteste = schuelerCollection
            //    .AsQueryable()
            //    .GroupBy(s => s.KlasseId)
            //    .Select(g => new
            //    {
            //        KlasseId = g.Key,
            //        Gebdat = g.Min(s => s.Gebdat)
            //    })
            //    .Join(schuelerCollection.AsQueryable(), s => new { s.KlasseId, s.Gebdat }, s => new { s.KlasseId, s.Gebdat }, (s1, s2) => s2)
            //    .ToList();

            // Man könnte 2x AsQueryable().ToList() verwenden, jedoch wird dann die gesamte Collection
            // übertragen. Daher senden wir eine selbst definierte Pipeline.

            var pipeline = PipelineDefinition<Schueler, ClassStatDto>.Create(
@"{
    '$group': {
      '_id': '$KlasseId',
      'Klasse': { '$first': '$KlasseId' },
      'MinGebdat': { '$min': '$Gebdat' }
    }
  }",
@"{
    '$lookup': {
      'from': 'Schueler',
      'let': {
        'klasse': '$_id',
        'minGebdat': '$MinGebdat'
      },
      'pipeline': [
        {
          '$match': {
            '$expr': {
              '$and': [
                { '$eq': [ '$$klasse', '$KlasseId' ] },
                { '$eq': [ '$$minGebdat', '$Gebdat' ] }
              ]
            }
          }
        }
      ],
      'as': 'Schueler'
    }
  }",
@"{
    '$sort': { 'Klasse': 1 }
}");

            var result = schuelerCollection.Aggregate(pipeline).ToList();
            foreach (var r in result)
            {
                Console.WriteLine($"Die ältesten Schüler der {r.Klasse} sind: ");
                foreach (var s in r.Schueler)
                {
                    Console.WriteLine($"   {s.Zuname} {s.Vorname}, geb. am {s.Gebdat:dd.MM.yyyy}");
                }
            }
        }
    }
}
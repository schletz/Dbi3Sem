using MongoDB.Driver;
using MongoDbDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbDemo.Infrastructure
{
    /// <summary>
    /// Stellt spezifische Methoden für die Collection Schueler bereit.
    /// </summary>
    class SchuelerRepository<TKey> : Repository<Schueler, TKey>
    {
        private readonly IMongoCollection<Schueler> _coll;
        private readonly Func<Schueler, TKey> _keySelector;

        public SchuelerRepository(IMongoCollection<Schueler> coll, Func<Schueler, TKey> keySelector) : base(coll, keySelector)
        {
            _coll = coll;
            _keySelector = keySelector;
        }

        public int GetSchuelerProKlasse(string klasse)
        {
            return _coll.AsQueryable().Count(s => s.KlasseId == klasse);
        }

        public Dictionary<string, int> GetSchuelerProKlasse()
        {
            return _coll
                .AsQueryable()
                .GroupBy(s => s.KlasseId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count).ThenBy(g => g.Id)
                .ToDictionary(g => g.Id, g => g.Count);
        }
        /// <summary>
        /// Liefert die volljährigen Schüler. Damit die Funktion deterministisch ist, wird das
        /// Datum übergeben.
        /// </summary>
        public List<Schueler> GetSchuelerVolljaehrig(DateTime today)
        {
            return _coll
                .AsQueryable()
                .Where(s => s.Gebdat <= today.AddYears(-18))
                .ToList();
        }
    }
}

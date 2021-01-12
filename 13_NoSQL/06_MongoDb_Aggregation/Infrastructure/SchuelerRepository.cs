using MongoDB.Driver;
using MongoDbDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbDemo.Infrastructure
{
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
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(g => g.Key, g => g.Count);
        }
    }
}

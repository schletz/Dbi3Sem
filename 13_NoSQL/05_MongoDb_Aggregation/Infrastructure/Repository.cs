using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MongoDbDemo.Infrastructure
{
    /// <summary>
    /// Stellt CRUD Operationen für eine Collection bereit.
    /// </summary>
    class Repository<TCollection, TKey>
    {
        IMongoCollection<TCollection> _coll;
        Func<TCollection, TKey> _keySelector;

        public Repository(IMongoCollection<TCollection> coll, Func<TCollection, TKey> keySelector)
        {
            _coll = coll ?? throw new ArgumentNullException(nameof(coll));
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public IMongoQueryable<TCollection> Find()
        {
            return _coll.AsQueryable();
        }
        public TCollection FindById(TKey id)
        {
            return _coll.Find(
                Builders<TCollection>.Filter.Eq("_id", id)
            ).FirstOrDefault();
        }
        public void Update(TCollection item)
        {
            var id = _keySelector(item);

            _coll.ReplaceOne(Builders<TCollection>.Filter.Eq("_id", id), item);
        }

        public void Delete(TCollection item)
        {
            var id = _keySelector(item);

            _coll.DeleteOne(Builders<TCollection>.Filter.Eq("_id", id));
        }
        public void Insert(TCollection item)
        {
            _coll.InsertOne(item);
        }
        public void InsertMany(IEnumerable<TCollection> items)
        {
            _coll.InsertMany(items);
        }
    }
}

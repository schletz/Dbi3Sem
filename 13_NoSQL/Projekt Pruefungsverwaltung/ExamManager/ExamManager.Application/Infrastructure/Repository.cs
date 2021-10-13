using ExamManager.Application.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public class Repository<TDocument, TKey>
    {
        protected readonly IMongoCollection<TDocument> _coll;
        private readonly Func<TDocument, TKey> _keySelector;
        public IMongoQueryable<TDocument> Queryable => _coll.AsQueryable();

        public Repository(IMongoCollection<TDocument> coll, Func<TDocument, TKey> keySelector)
        {
            _coll = coll;
            _keySelector = keySelector;
        }
        public void InsertOne(TDocument element) => _coll.InsertOne(element);
        public void DeleteOne(TKey id) => _coll.DeleteOne(Builders<TDocument>.Filter.Eq("_id", id));
        public void UpdateOne(TDocument element)
        {
            _coll.ReplaceOne(Builders<TDocument>.Filter.Eq("_id", _keySelector(element)), element);
        }
        public TDocument? FindById(TKey id)
        {
            var result = _coll.Find(Builders<TDocument>.Filter.Eq("_id", id));
            return result.FirstOrDefault();
        }
        public void DeleteAll()
        {
            _coll.DeleteMany(Builders<TDocument>.Filter.Empty);
        }
    }
}

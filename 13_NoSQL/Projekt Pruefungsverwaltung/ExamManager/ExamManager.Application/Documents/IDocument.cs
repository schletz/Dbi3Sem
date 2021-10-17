using MongoDB.Bson.Serialization.Attributes;

namespace ExamManager.Application.Documents
{
    public interface IDocument<TKey>
    {
        [BsonId]
        public TKey Id { get; }
    }
}
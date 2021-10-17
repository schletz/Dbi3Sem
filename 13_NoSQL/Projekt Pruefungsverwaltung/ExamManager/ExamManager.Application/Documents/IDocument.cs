using MongoDB.Bson.Serialization.Attributes;

namespace ExamManager.Application.Documents
{
    /// <summary>
    /// Interface für alle Dokumente, die in einer Collection gespeichert werden sollen.
    /// Definiert einen Key für das generische Repository.
    /// </summary>
    public interface IDocument<TKey>
    {
        [BsonId]
        public TKey Id { get; }
    }
}
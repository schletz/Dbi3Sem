using MongoDB.Bson.Serialization.Attributes;

namespace ExamDbGenerator.Model
{
    record Room([property: BsonId] string Shortname, int? Capacity = null);

}

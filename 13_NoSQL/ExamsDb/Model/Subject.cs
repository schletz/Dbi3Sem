using MongoDB.Bson.Serialization.Attributes;

namespace ExamDbGenerator.Model
{
    record Subject([property: BsonId] string Shortname, string Longname);

}

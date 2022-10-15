using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ExamDbGenerator.Model
{
    record Term(int Year, [property: BsonRepresentation(BsonType.String)] TermType TermType, DateOnly Start, DateOnly End)
    {
        [BsonElement]
        public string Id => TermType == TermType.Year ? Year.ToString() : $"{Year}{TermType.ToString()[0]}";
    }

}

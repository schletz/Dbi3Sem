using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ExamDbGenerator.Model
{

    record Teacher(
        TeacherName Name,
        [property: BsonRepresentation(BsonType.String)] Gender Gender,
        int? HoursPerWeek = null, TimeOnly? LessonsFrom = null,
        // Bei LINQ Berechnungen wie Average, ... würden wir ohne AllowTruncation eine Exception bekommen.
        [property:BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        decimal? Salary = null)
    {
        [BsonId]
        public string Id => Name.Shortname;
        [BsonElement]
        public List<string> HomeOfficeDays { get; set; } = new();
        [BsonElement]
        public List<Subject> CanTeachSubjects { get; set; } = new();
    }

}

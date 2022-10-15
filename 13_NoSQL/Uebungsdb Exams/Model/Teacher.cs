using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ExamDbGenerator.Model
{
    record Teacher(TeacherName Name, decimal? Salary)
    {
        [BsonId]
        public string Id => Name.Shortname;
        [BsonElement]
        public List<string> HomeOfficeDays { get; set; } = new();
        [BsonElement]
        public List<Subject> CanTeachSubjects { get; set; } = new();
    }

}

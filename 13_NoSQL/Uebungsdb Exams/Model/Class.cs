using MongoDB.Bson.Serialization.Attributes;

namespace ExamDbGenerator.Model
{
    record Class(string Shortname, Term Term, string Department, int EducationLevel, string Letter, TeacherName ClassTeacher, string? RoomShortname = null)
    {
        [BsonElement]
        public string Id => $"{Term.Id}_{Shortname}";
    }

}

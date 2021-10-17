using ExamManager.Application.Documents;
using MongoDB.Driver;

namespace ExamManager.Application.Infrastructure
{
    public class TeacherRepository : Repository<Teacher, string>
    {
        public TeacherRepository(IMongoCollection<Teacher> coll) : base(coll)
        {
        }
    }
}
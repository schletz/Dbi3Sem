using ExamManager.Application.Documents;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public class StudentRepository
    {
        private readonly IMongoCollection<Student> _coll;

        public StudentRepository(IMongoCollection<Student> coll)
        {
            _coll = coll;
        }

        public Student? GetStudentById(long id)
        {
            return _coll.AsQueryable().FirstOrDefault(s => s.Id == id);
        }

        public void InsertStudent(Student s)
        {
            _coll.InsertOne(s);
        }

        public void UpdateStudent(Student s)
        {
            _coll.ReplaceOne(Builders<Student>.Filter.Eq("_id", s.Id), s);
        }

        public void DeleteStudent(long id)
        {
            _coll.DeleteOne(Builders<Student>.Filter.Eq("_id", id));
        }

        public void DeleteAllStudents()
        {
            _coll.DeleteMany(Builders<Student>.Filter.Empty);
        }
    }
}

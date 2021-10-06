using ExamManager.Application.Documents;
using ExamManager.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExamManager.Test
{
    public class StudentRepositoryTest
    {
        [Fact]
        public void InsertStudentTest()
        {
            var s = new Student(
                id: 4,
                firstname: "FN1",
                lastname: "LN1",
                dateOfBirth: new DateTime(2002, 12, 31));
            var manager = new RepositoryManager("127.0.0.1", "Exams");
            //var repo = manager.GetRepository<Student, long>(s => s.Id);
            var repo = manager.GetRepository((Student s) => s.Id);
            repo.DeleteAll();
            repo.InsertOne(s);
            var s2 = repo.FindById(s.Id);
            Assert.True(s.Id == s2.Id);
            Assert.True(s.Guid == s2.Guid);
        }
    }
}

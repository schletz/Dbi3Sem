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
            var s1 = new Student(
                id: 4,
                firstname: "FN1",
                lastname: "LN1",
                schoolClass: "5AHIF",
                dateOfBirth: new DateTime(2002, 12, 31));
            var manager = new RepositoryManager("127.0.0.1", "Exams");
            var repo = manager.GetRepository<Student, long>(s => s.Id);

            repo.DeleteAll();
            repo.InsertOne(s1);
            var s2 = repo.Queryable
                .FirstOrDefault(s => s.Id == s1.Id);
            Assert.True(s1.Id == s2.Id);
            Assert.True(s1.Guid == s2.Guid);
        }
    }
}
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
                id: 1,
                firstname: "FN1",
                lastname: "LN1",
                dateOfBirth: new DateTime(2002, 12, 31));
            var manager = new RepositoryManager("127.0.0.1", "Exams");
            var repo = manager.GetStudentRepository();
            repo.DeleteAllStudents();
            repo.InsertStudent(s);
            var s2 = repo.GetStudentById(s.Id);
            Assert.True(s.Id == s2.Id);
        }
    }
}

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
    public class RepositoryManagerTests
    {
        [Fact]
        public void SeedDatabaseTest()
        {
            var manager = new RepositoryManager("127.0.0.1", "Exams");
            manager.Seed();
            var repo = manager.GetRepository<Student, long>(s => s.Id);
            Assert.True(repo.Queryable.Any());
        }
    }
}
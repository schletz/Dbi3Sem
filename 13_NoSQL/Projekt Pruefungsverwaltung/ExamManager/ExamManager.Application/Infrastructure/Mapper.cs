using ExamManager.Application.Documents;
using ExamManager.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public static class Mapper
    {
        public static StudentDto Map(Student s)
        {
            return new StudentDto(
                s.Id,
                s.Firstname,
                s.Lastname,
                s.SchoolClass,
                s.DateOfBirth,
                s.Grades.ToDictionary(g => g.Key, g => Map(g.Value)));
        }

        public static GradeDto Map(Grade g)
        {
            return new GradeDto(g.Value, g.Subject);
        }
    }
}

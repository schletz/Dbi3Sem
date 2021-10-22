using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Dto
{
    public record GradeDto(
        int Value,
        string Subject);
    public record StudentDto(
        long Id,
        string Firstname,
        string Lastname,
        string SchoolClass,
        DateTime DateOfBirth,
        Dictionary<string, GradeDto> Grades);
}

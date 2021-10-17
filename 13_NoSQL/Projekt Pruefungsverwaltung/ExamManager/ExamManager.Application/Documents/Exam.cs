using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Documents
{
    public class Exam
    {
        public Exam(Student student, Teacher teacher, string subject)
        {
            StudentId = student.Id;
            StudentFirstname = student.Firstname;
            StudentLastname = student.Lastname;
            StudentDateOfBirth = student.DateOfBirth;
            StudentSchoolClass = student.SchoolClass; ;
            Teacher = teacher;
            Subject = subject;
            DateCrated = DateTime.UtcNow;
        }

        /// <summary>
        /// Copyconstruktor zum Anlegen eines GradesExams
        /// </summary>
        protected Exam(Exam e)
        {
            Id = e.Id;
            StudentId = e.StudentId;
            StudentFirstname = e.StudentFirstname;
            StudentLastname = e.StudentLastname;
            StudentDateOfBirth = e.StudentDateOfBirth;
            StudentSchoolClass = e.StudentSchoolClass;
            Teacher = e.Teacher;
            Subject = e.Subject;
            DateCrated = e.DateCrated;
        }

        public Guid Id { get; private set; }
        public long StudentId { get; set; }
        public string StudentFirstname { get; set; }
        public string StudentLastname { get; set; }
        public DateTime StudentDateOfBirth { get; set; }
        public string StudentSchoolClass { get; set; }
        public Teacher Teacher { get; set; }
        public string Subject { get; set; }

        /// <summary>
        /// Berechnetes Feld, deswegen verbieten wir den Schreibzugriff mit private set.
        /// </summary>
        public DateTime DateCrated { get; private set; }
    }
}
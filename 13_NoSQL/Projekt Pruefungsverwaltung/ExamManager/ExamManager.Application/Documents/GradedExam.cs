using System;

namespace ExamManager.Application.Documents
{
    /// <summary>
    /// Beurteilte Prüfung.
    /// </summary>
    public class GradedExam : Exam
    {
        public GradedExam(Exam exam, Teacher assistant, int grade)
            : base(exam)
        {
            Assistant = assistant;
            Grade = grade;
            DateGraded = DateTime.UtcNow;
        }

        public Teacher Assistant { get; set; }
        public int Grade { get; set; }
        public DateTime DateGraded { get; private set; }
    }
}
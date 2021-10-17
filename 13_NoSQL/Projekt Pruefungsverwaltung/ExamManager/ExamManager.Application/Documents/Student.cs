using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Documents
{
    /// <summary>
    /// Speichert die Daten eines Schülers in einer Collection.
    /// </summary>
    public class Student : IDocument<long>
    {
        /// <summary>
        /// Konstruktor ohne GUID. Sie wird automatisch generiert.
        /// </summary>
        public Student(long id,
            string firstname,
            string lastname,
            string schoolClass,
            DateTime dateOfBirth)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            SchoolClass = schoolClass;
            DateOfBirth = dateOfBirth;
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Backing field für das Dictionary Grades. Wird in die DB gemappt.
        /// </summary>
        [BsonElement("Grades")]
        private readonly Dictionary<string, Grade> _grades = new Dictionary<string, Grade>(0);

        /// <summary>
        /// Schülernummer (mehr als 9 Stellen lang, deswegen ein long Wert)
        /// </summary>
        public long Id { get; private set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string SchoolClass { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateOfBirth { get; set; }

        public Guid Guid { get; set; }

        /// <summary>
        /// Externer Zugriff auf die Noten. Wird nicht in die DB gemappt.
        /// </summary>
        [BsonIgnore]             // Optional, wird automatisch angenommen.
        public IReadOnlyDictionary<string, Grade> Grades => _grades;

        /// <summary>
        /// Für den leichteren Zugriff auf die negativen Noten liefern wird diese zurück.
        /// Wird nicht in der Datenbank gespeichert.
        /// </summary>
        [BsonIgnore]
        public IEnumerable<Grade> NegativeGrades => Grades.Values.Where(g => g.Value == 5);

        /// <summary>
        /// Damit positiv abgeschlossene Schüler über die Datenbank leicht ausgelesen werden können,
        /// muss [BsonElement] bei diesem read only property hinzugefügt werden.
        /// </summary>
        [BsonElement]
        public bool IsPositive => !NegativeGrades.Any();

        public bool CalcAufstieg(IEnumerable<GradedExam> exams) => !GradesAfterExam(exams).Any(g => g.Value == 5);

        public IEnumerable<Grade> GradesAfterExam(IEnumerable<GradedExam> exams) =>
            _grades
                .GroupJoin(exams, g => g.Key, e => e.Subject, (grade, exams) =>
                        exams.FirstOrDefault()?.Grade ?? grade.Value);

        public void UpsertGrade(Grade g)
        {
            if (_grades.TryGetValue(g.Subject, out var existing))
            {
                existing.Value = g.Value;
                existing.Updated = DateTime.UtcNow;
                return;
            }
            _grades.Add(g.Subject, g);
        }
    }
}
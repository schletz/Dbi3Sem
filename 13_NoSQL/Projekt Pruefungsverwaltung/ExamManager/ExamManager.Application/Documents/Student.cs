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
    public class Student
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
        /// Schülernummer (mehr als 9 Stellen lang, deswegen ein long Wert)
        /// </summary>
        public long Id { get; private set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string SchoolClass { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateOfBirth { get; set; }

        public Guid Guid { get; set; }
        public Dictionary<string, Grade> Grades { get; private set; } = new(0);

        /// <summary>
        /// Damit der Aufstieg über Filterfunktionen von der Datenbank ermittelt werden kann,
        /// muss [BsonElement] bei diesem read only property hinzugefügt werden.
        /// </summary>
        [BsonElement]
        public bool Aufstieg => !Grades.Values.Any(g => g.Value == 5);

        /// <summary>
        /// Für den leichteren Zugriff auf die negativen Noten liefern wird diese zurück.
        /// Wird nicht in der Datenbank gespeichert.
        /// </summary>
        [BsonIgnore]
        public IEnumerable<Grade> NegativeGrades => Grades.Values.Where(g => g.Value == 5);

        public void UpsertGrade(Grade g)
        {
            if (Grades.TryGetValue(g.Subject, out var existing))
            {
                existing.Value = g.Value;
                existing.Updated = DateTime.UtcNow;
                return;
            }
            Grades.Add(g.Subject, g);
        }
    }
}
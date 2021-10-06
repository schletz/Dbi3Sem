using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Documents
{
    public class Student
    {
        /// <summary>
        /// Konstruktor ohne GUID. Sie wird automatisch generiert.
        /// </summary>
        public Student(long id, 
            string firstname, 
            string lastname,
            DateTime dateOfBirth)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            DateOfBirth = dateOfBirth;
            Guid = Guid.NewGuid();
        }

        public long Id { get; private set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid Guid { get; set; }
    }
}

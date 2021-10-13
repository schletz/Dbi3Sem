﻿using MongoDB.Bson.Serialization.Attributes;
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
        public Dictionary<string, Grade> Grades { get; private set; } = new(0);
        [BsonElement]   // Schreibe den Aufstieg auch in die DB
        public bool Aufstieg => !Grades.Values.Any(g => g.Value == 5);
        public void UpsertGrade(Grade g)
        {
            if (Grades.TryGetValue(g.Subject, out var existing))
            {
                existing.Value = g.Value;
                existing.Updated = g.Updated = DateTime.UtcNow;
                return;
            }
            Grades.Add(g.Subject, g);
        }
    }
}

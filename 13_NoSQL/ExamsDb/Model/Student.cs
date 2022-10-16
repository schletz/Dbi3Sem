using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ExamDbGenerator.Model
{
    class Student
    {
        public Student(StudentName name, Gender gender, Address address, DateOnly dateOfBirth, Class? currentClass = null)
        {
            Name = name;
            CurrentClass = currentClass;
            Gender = gender;
            Address = address;
            DateOfBirth = dateOfBirth;
        }

        [BsonId]
        public int Id => Name.Nr;
        public StudentName Name { get; set; }
        public Class? CurrentClass { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Gender Gender { get; set; }
        public Address Address { get; set; }
        public DateOnly DateOfBirth { get; set; }
        [BsonElement]
        public List<Class> ClassHistory { get; } = new();
    }


}

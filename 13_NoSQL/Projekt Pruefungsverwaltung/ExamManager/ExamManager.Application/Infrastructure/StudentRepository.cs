﻿using ExamManager.Application.Documents;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public class StudentRepository : Repository<Student, long>
    {
        public StudentRepository(IMongoCollection<Student> coll, Func<Student, long> keySelector) : base(coll, keySelector)
        {
        }

        public void InsertGrade(int grade)
        {
        }
    }
}

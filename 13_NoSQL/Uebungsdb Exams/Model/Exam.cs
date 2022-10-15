﻿using MongoDB.Bson;
using System;

namespace ExamDbGenerator.Model
{
    record Exam(StudentName Student, TeacherName Teacher, Class ExamClass, Subject Subject, DateTime DateTime, int PointsMax, int Points, int Grade)
    {
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();
    }

}

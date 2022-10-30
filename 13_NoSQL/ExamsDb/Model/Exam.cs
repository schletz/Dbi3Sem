using MongoDB.Bson;
using System;

namespace ExamDbGenerator.Model
{
    record Exam(
        StudentName Student, TeacherName Teacher, Class CurrentClass, Class ExamClass,
        Subject Subject, DateTime DateTime, int PointsMax, int Points, int Grade, ObjectId Id = default);

}

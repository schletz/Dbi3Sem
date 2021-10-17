using Bogus;
using Bogus.Extensions;
using ExamManager.Application.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Infrastructure
{
    public class ExamDatabase
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        public bool EnableLogging { get; set; }

        public ExamDatabase(string host, string database)
        {
            var settings = new MongoClientSettings
            {
#if DEBUG
                ClusterConfigurator = cb =>
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        if (EnableLogging) Debug.WriteLine(e.Command.ToString());
                    }),
#endif
                Server = new MongoServerAddress(host)
            };

            _client = new MongoClient(settings);
            _db = _client.GetDatabase(database);
        }

        public Repository<Exam, Guid> ExamRepository => new Repository<Exam, Guid>(_db.GetCollection<Exam>(nameof(Exam)));
        public StudentRepository StudentRepository => new StudentRepository(_db.GetCollection<Student>(nameof(Student)));
        public TeacherRepository TeacherRepository => new TeacherRepository(_db.GetCollection<Teacher>(nameof(Teacher)));

        public void Seed()
        {
            _db.DropCollection(nameof(Student));
            _db.DropCollection(nameof(Exam));
            _db.DropCollection(nameof(Teacher));

            Randomizer.Seed = new Random(1454);
            var rnd = new Randomizer();

            var teachers = new Faker<Teacher>("de")
                .CustomInstantiator(f =>
                {
                    var lastname = f.Name.LastName();
                    return new Teacher(
                        shortname: lastname.Length < 3 ? lastname.ToUpper() : lastname.Substring(0, 3).ToUpper(),
                        firstname: f.Name.FirstName(),
                        lastname: lastname)
                    {
                        Email = $"{lastname.ToLower()}@spengergasse.at".OrDefault(f, 0.25f) // using Bogus.Extensions;
                    };
                })
                .Generate(200)       // Take nimmt nur 20 Elemente des Enumerators
                .GroupBy(g => g.Shortname)
                .Select(g => g.First())
                .Take(20)
                .ToList();
            _db.GetCollection<Teacher>(nameof(Teacher)).InsertMany(teachers);

            int id = 1000;
            var subjects = new string[] { "POS", "DBI", "D", "E", "AM" };
            var classes = new string[] { "4AHIF", "4BHIF", "4CHIF", "4EHIF" };
            var students = new Faker<Student>("de")
                .CustomInstantiator(f =>
                {
                    var s = new Student(
                        id: id++,
                        firstname: f.Name.FirstName(),
                        lastname: f.Name.LastName(),
                        schoolClass: f.Random.ListItem(classes),
                        dateOfBirth: new DateTime(2003, 1, 1).AddDays(f.Random.Int(0, 4 * 365)));

                    var grades = new Faker<Grade>("de")
                        .CustomInstantiator(f =>
                            new Grade(
                                value: f.Random.Int(1, 5),
                                subject: f.Random.ListItem(subjects)))
                        .Generate(5);
                    foreach (var g in grades)
                    {
                        s.UpsertGrade(g);
                    }
                    return s;
                })
                .Generate(100)
                .ToList();
            _db.GetCollection<Student>(nameof(Student)).InsertMany(students);

            var negative = students.SelectMany(s => s.NegativeGrades.Select(n => new { Student = s, Grade = n })).ToList();
            var exams = rnd
                        .ListItems(negative, negative.Count / 2)
                        .Select(n =>
                        {
                            var teacher = rnd.ListItem(teachers);
                            var assistant = rnd.ListItem(teachers);
                            // In 20% der Fälle liefern wir ein GradedExam (schon benotet).
                            var e = new Exam(
                                student: n.Student,
                                teacher: rnd.ListItem(teachers),
                                subject: n.Grade.Subject);
                            return rnd.Bool(0.5f) && teacher.Shortname != assistant.Shortname
                                ? new GradedExam(
                                    exam: e,
                                    assistant: rnd.ListItem(teachers),
                                    grade: new Grade(value: rnd.Int(3, 5), subject: n.Grade.Subject))
                                : e;
                        })
                        .ToList();
            _db.GetCollection<Exam>(nameof(Exam)).InsertMany(exams);
        }
    }
}
﻿using Bogus;
using DnsClient;
using ExamDbGenerator.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamDbGenerator
{
    /// <summary>
    /// Unit of Work für die Exam Datenbank. Stellt alle Collections als Property bereit,
    /// damit auf diese zugegriffen werden kann. Änderungen bei den Namen können so sehr einfach
    /// gemacht werden.
    /// </summary>
    class ExamDatabase
    {
        public MongoClient Client { get; }
        public IMongoDatabase Db { get; }
        public IMongoCollection<Term> Terms => Db.GetCollection<Term>("terms");
        public IMongoCollection<Subject> Subjects => Db.GetCollection<Subject>("subjects");
        public IMongoCollection<Room> Rooms => Db.GetCollection<Room>("rooms");
        public IMongoCollection<Class> Classes => Db.GetCollection<Class>("classes");
        public IMongoCollection<Student> Students => Db.GetCollection<Student>("students");
        public IMongoCollection<Teacher> Teachers => Db.GetCollection<Teacher>("teachers");
        public IMongoCollection<Exam> Exams => Db.GetCollection<Exam>("exams");

        /// <summary>
        /// Konfiguriert die Datenbank für einen Connection string.
        /// </summary>
        public static ExamDatabase FromConnectionString(string connectionString, bool logging = false)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            if (logging)
            {
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        // Bei update Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("updates", out var updateCmd))
                        {
                            var collection = e.Command.GetValue("update");
                            var isUpdateOne = updateCmd[0]["q"].AsBsonDocument.Contains("_id");
                            foreach (var cmd in updateCmd.AsBsonArray)
                            {
                                Console.WriteLine($"db.getCollection(\"{collection}\").{(isUpdateOne ? "updateOne" : "updateMany")}({updateCmd[0]["q"]}, {updateCmd[0]["u"]})");
                            }
                        }
                        // Bei aggregate Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("aggregate", out var aggregateCmd))
                        {
                            var collection = aggregateCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").aggregate({e.Command["pipeline"]})");
                        }

                        // Bei Filter Statements geben wir die find Anweisung aus.
                        if (e.Command.TryGetValue("find", out var findCmd))
                        {
                            var collection = findCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").find({e.Command["filter"]})");
                        }
                    });
                };
            }
            var client = new MongoClient(settings);
            var db = client.GetDatabase("examsDb");
            BsonSerializer.RegisterSerializer(typeof(DateOnly), new DateOnlySerializer());
            BsonSerializer.RegisterSerializer(typeof(TimeOnly), new TimeOnlySerializer());
            // LowerCase property names.
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(ignoreIfNull: true)
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
            return new ExamDatabase(client, db);
        }

        private ExamDatabase(MongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;
        }

        /// <summary>
        /// Löscht die Datenbank und befüllt sie mit Musterdaten.
        /// </summary>
        public void Seed()
        {
            Db.DropCollection("terms");
            Db.DropCollection("subjects");
            Db.DropCollection("rooms");
            Db.DropCollection("classes");
            Db.DropCollection("students");
            Db.DropCollection("teachers");
            Db.DropCollection("exams");

            Randomizer.Seed = new Random(1109);
            var faker = new Faker("de");

            var terms = Enumerable.Range(2019, 11).SelectMany(year => new Term[]
            {
                new Term(Year: year, TermType: TermType.Winter, Start: new DateOnly(year,9,1), End: new DateOnly(year+1,2,1)),
                new Term(Year: year, TermType: TermType.Summer, Start: new DateOnly(year+1,2,1), End: new DateOnly(year+1,7,1)),
            })
            .ToList();
            Terms.InsertMany(terms);

            var subjects = new List<Subject>()
            {
                new Subject(Shortname: "AM", Longname: "Angewandte Mathematik"),
                new Subject(Shortname: "DBI", Longname: "Datenbanken und Informationssysteme"),
                new Subject(Shortname: "D", Longname: "Deutsch"),
                new Subject(Shortname: "POS", Longname: "Programmieren und Software Engineering"),
            };
            Subjects.InsertMany(subjects);

            var rooms = new Faker<Room>("de").CustomInstantiator(f =>
            {
                var building = f.Random.String2(1, "ABC");
                var floor = f.Random.String2(1, "12345H");

                return new Room(
                    Shortname: $"{building}{floor}.{f.Random.Int(1, 14):00}",
                    Capacity: f.Random.Int(16, 32).OrNull(f, 0.4f));
            })
            .Generate(20)
            .GroupBy(r => r.Shortname).Select(g => g.First())
            .ToList();
            Rooms.InsertMany(rooms);

            var teachers = new Faker<Teacher>("de").CustomInstantiator(f =>
            {
                var gender = f.Person.Gender;
                var lastname = f.Name.LastName();
                var shortname = lastname.Substring(0, Math.Min(3, lastname.Length)).ToUpper();
                return new Teacher(
                    Name: new TeacherName(Shortname: shortname, Firstname: f.Name.FirstName(), Lastname: lastname, Email: $"{lastname.ToLower()}@spengergasse.at"),
                    Gender: gender == Bogus.DataSets.Name.Gender.Female ? Gender.Female : Gender.Male,
                    HoursPerWeek: f.Random.Int(6, 18).OrNull(f, 0.5f),
                    LessonsFrom: new TimeOnly(f.Random.Int(12, 17), 0, 0).OrNull(f, 0.5f),
                    Salary: (f.Random.Int(300000, 450000) / 100M).OrNull(f, 0.4f))
                {
                    HomeOfficeDays = f.Random.ListItems(new string[] { "MO", "DI", "MI", "DO", "FR" }, f.Random.Int(0, 2)),
                    CanTeachSubjects = f.Random.ListItems(subjects, f.Random.Int(0, 2)).ToList()
                };
            })
            .Generate(30)
            .GroupBy(r => r.Id).Select(g => g.First())
            .ToList();
            Teachers.InsertMany(teachers);


            var departments = new string[] { "AIF", "KIF", "BIF", "CIF" };
            var maxEducationLevel = new Dictionary<string, int>
            {
                {"AIF", 6 },{"KIF", 6 },{"BIF", 8 },{"CIF", 8 },
            };
            var minEducationLevel = new Dictionary<string, int>
            {
                {"AIF", 2 },{"KIF", 3 },{"BIF", 2 },{"CIF", 3 },
            };

            var classes = new List<Class>();
            foreach (var term in terms.Where(t => t.Year <= 2022))
                foreach (var department in departments)
                    for (int educationLevel = minEducationLevel[department]; educationLevel <= maxEducationLevel[department]; educationLevel++)
                    {
                        if (term.TermType == TermType.Summer && educationLevel % 2 == 1) { continue; }
                        if (term.TermType == TermType.Winter && educationLevel % 2 == 0) { continue; }
                        foreach (var letter in new string[] { "A", "B" })
                        {
                            // Aufsteigender Studienkoordinator.
                            var classTeacher = classes.FirstOrDefault(c =>
                                    c.Department == department && c.Letter == letter && c.EducationLevel == educationLevel - 1 &&
                                    c.Term.TermType == (term.TermType == TermType.Summer ? TermType.Winter : TermType.Summer) &&
                                    c.Term.Year == (term.TermType == TermType.Summer ? term.Year : term.Year - 1))?.ClassTeacher
                                ?? faker.Random.ListItem(teachers).Name;
                            classes.Add(new Class(
                                Shortname: $"{educationLevel}{letter}{department}",
                                Term: term,
                                Department: department,
                                EducationLevel: educationLevel,
                                Letter: letter,
                                ClassTeacher: classTeacher,
                                RoomShortname: faker.Random.ListItem(rooms).Shortname.OrNull(faker, 0.25f)));
                        }
                    }
            Classes.InsertMany(classes);
            Classes.Indexes.CreateOne(new CreateIndexModel<Class>(Builders<Class>.IndexKeys.Ascending(c => c.Term.Year)));

            var students = new List<Student>();
            var maxBirthdate = new Dictionary<string, DateOnly>()
            {
                {"AIF", new DateOnly(2005,9,1) },{"KIF", new DateOnly(2000,9,1) },{"BIF", new DateOnly(1998,9,1) },{"CIF", new DateOnly(1995,9,1) }
            };
            // Neue Studierende in den Anfangssemester Klassen generieren
            int studentId = 100001;
            foreach (var @class in classes.Where(c => c.EducationLevel == minEducationLevel[c.Department]))
            {
                students.AddRange(new Faker<Student>("de").CustomInstantiator(f =>
                {
                    var gender = f.Person.Gender;
                    var student = new Student(
                        name: new StudentName(Nr: studentId++, Firstname: f.Name.FirstName(gender), Lastname: f.Name.LastName(gender)),
                        gender: gender == Bogus.DataSets.Name.Gender.Female ? Gender.Female : Gender.Male,
                        address: new Address(Street: f.Address.StreetName(), StreetNr: f.Address.BuildingNumber(), City: f.Address.City(), Zip: f.Random.Int(100, 299) * 10),
                        dateOfBirth: maxBirthdate[@class.Department].AddDays(-1 * f.Random.Int(0, 5 * 365)),
                        currentClass: @class);
                    student.ClassHistory.Add(@class);
                    return student;
                })
                .Generate(faker.Random.Int(20, 30)));
            }

            // "Aufsteigen" in die ClassHistory eintragen.
            foreach (var @class in classes.Where(c => c.Term.Year <= 2022 &&
                c.EducationLevel > minEducationLevel[c.Department]).OrderBy(c => c.EducationLevel))
            {
                var lastClass = classes.FirstOrDefault(c => c.Department == @class.Department && c.Letter == @class.Letter &&
                    c.EducationLevel == @class.EducationLevel - 1 &&
                    c.Term.TermType == (@class.Term.TermType == TermType.Summer ? TermType.Winter : TermType.Summer) &&
                    c.Term.Year == (@class.Term.TermType == TermType.Summer ? @class.Term.Year : @class.Term.Year - 1));
                if (lastClass is null) { continue; }
                var lastTermStudents = students.Where(s => s.CurrentClass!.Id == lastClass.Id).ToList();
                foreach (var student in @class.Term.TermType == TermType.Summer ? lastTermStudents : faker.Random.ListItems(lastTermStudents, (int)(lastTermStudents.Count * 0.8)))
                {
                    student.CurrentClass = @class;
                    student.ClassHistory.Add(@class);
                }
            }

            int examsCount = 1;
            var exams = new Faker<Exam>("de").CustomInstantiator(f =>
            {
                // Nur Students, die letztes Jahr auch da waren, können Prüfungen haben.
                var student = f.Random.ListItem(students.Where(s => s.ClassHistory.Count() > 1).ToList());
                var subject = f.Random.ListItem(subjects);
                var teacher = f.Random.ListItem(teachers.Where(t => !t.CanTeachSubjects.Any() || t.CanTeachSubjects.Any(s => s.Shortname == subject.Shortname)).ToList());
                var examClass = f.Random.ListItem(student.ClassHistory.Where(c => c.Id != student.CurrentClass!.Id).ToList());
                var currentClass = f.Random.ListItem(student.ClassHistory.Where(c => c.Term.Start >= examClass.Term.End).ToList());
                var pointsmax = f.Random.Int(16, 48);
                var points = f.Random.Int((int)(pointsmax * 0.25), pointsmax);
                return new Exam(
                    Student: student.Name,
                    Teacher: f.Random.ListItem(teachers).Name,
                    CurrentClass: currentClass,
                    ExamClass: examClass,
                    Subject: subject,
                    DateTime: examClass.Term.End.AddDays(f.Random.Int(0, 365)).ToDateTime(new TimeOnly(f.Random.Int(8, 20), 0, 0)),
                    PointsMax: pointsmax,
                    Points: points,
                    Grade: Math.Min(5, 9 - (int)Math.Ceiling(8M * points / pointsmax)),
                    // Deterministische ObjectId erzeugen (Zahl als 12 Byte Hexstring wird einfach geparst)
                    Id: new ObjectId(examsCount++.ToString("x24")));
            })
            .Generate((int)(students.Count * 0.5))
            .ToList();
            Exams.InsertMany(exams);
            Exams.Indexes.CreateOne(new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(c => c.CurrentClass.Id)));
            Exams.Indexes.CreateOne(new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(c => c.Student.Nr)));
            Exams.Indexes.CreateOne(new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(c => c.Teacher.Shortname)));

            // Zum Schluss die CurrentClass des Students auf den Wert von 2022/23 setzen. Das machen
            // wir nach der Exam Generierung, da die CurrentClass davor "aufsteigend" gesetzt wird.
            foreach (var student in students)
            {
                if (student.CurrentClass!.Term.Year != 2022) { student.CurrentClass = null; }
            }
            Students.InsertMany(students);
            Students.Indexes.CreateOne(new CreateIndexModel<Student>(Builders<Student>.IndexKeys.Ascending(c => c.CurrentClass!.Shortname)));

        }
    }
}

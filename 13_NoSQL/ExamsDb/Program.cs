using ExamDbGenerator;
using MongoDB.Driver;
using System;
using ExamDbGenerator.Model;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

class Program
{
    static int Main(string[] args)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Clear();

        var examsDb = ExamDatabase.FromConnectionString("mongodb://root:1234@localhost:27017", logging: false);
        try
        {
            examsDb.Seed();
        }
        catch (TimeoutException)
        {
            Console.Error.WriteLine("Die Datenbank ist nicht erreichbar. Läuft der Container?");
            return 1;
        }
        catch (MongoAuthenticationException)
        {
            Console.Error.WriteLine("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
            return 2;
        }

        Console.WriteLine("Die Datenbank ExamsDb wurde angelegt. Du kannst dich nun im MongoDb Compass mit dem connection string");
        Console.WriteLine("    mongodb://root:1234@localhost:27017");
        Console.WriteLine("verbinden.");
        Console.WriteLine();
        Console.WriteLine("Übersicht der Collections:");
        Console.WriteLine($"    {examsDb.Classes.CountDocuments("{}")} Dokumente in der Collection Classes.");
        Console.WriteLine($"    {examsDb.Exams.CountDocuments("{}")} Dokumente in der Collection Exams.");
        Console.WriteLine($"    {examsDb.Rooms.CountDocuments("{}")} Dokumente in der Collection Rooms.");
        Console.WriteLine($"    {examsDb.Students.CountDocuments("{}")} Dokumente in der Collection Students.");
        Console.WriteLine($"    {examsDb.Subjects.CountDocuments("{}")} Dokumente in der Collection Subjects.");
        Console.WriteLine($"    {examsDb.Teachers.CountDocuments("{}")} Dokumente in der Collection Teachers.");
        Console.WriteLine($"    {examsDb.Terms.CountDocuments("{}")} Dokumente in der Collection Terms.");
        return 0;
    }

}



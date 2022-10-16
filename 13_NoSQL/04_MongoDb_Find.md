# Filtern von Collections

So wie in jeder anderen Datenbank ist in MongoDB das Filtern von Daten ein zentraler Punkt. Wir
haben mehrere Möglichkeiten, Daten zu filtern:

- In der Mongo Shell
- Im MongoDb Compass
- In der Applikation mittels dem MongoDB Treiber

## Absetzen von Anfragen

### Filtern in der Shell

Mit Docker Desktop kannst du mit der Option *Open in terminal* eine Shell öffnen:

![](docker_terminal_0825.png)

Gib danach die folgendne Befehle ein. Im Connectionstring wird davon ausgegangen, dass der
User wie im Kapitel Installation beschrieben auf *root* mit dem Passwort *1234* gesetzt wurde:

```
/usr/bin/mongosh mongodb://root:1234@localhost:27017
```

Nun kann in der Shell direkt gearbeitet werden. Um alle Dokumente der Collection Klasse
anzeigen zu können, werden folgende Befehle verwendet:

```
use examsDb
db.getCollection("rooms").find({})
```

### Filtern im MongoDb Compass

Nachdem du dich mit der Datenbank im MongoDb Compass verbunden hast, kann im Feld *Filter*
ein Filterausdruck eingegeben werden. Das Ergebnis wird dann darunter angezeigt:

![](compass_filter_1340.png)

### Filtern im .NET MongoDB Treiber

#### Mit dem Builder

Der Builder generiert einen Suchfilter mit dem entsprechenden Operator. Im folgenden Beispiel
wird nach dem Raum mit dem Shortname A2.14 gesucht:

```c#
var filter = Builders<Room>.Filter.Eq(r => r.Shortname, "A2.14");
var results = db.GetCollection<Room>("rooms")
    .Find(filter);
Console.WriteLine(results);                     // Gibt den Suchfilter aus
foreach (var result in results.ToList())
{
    Console.WriteLine($"{result.Shortname} hat eine Kapazität von {result.Capacity} Pl#tzen.");
}

```

*Eq* ist der Operator (Equals). Er bekommt 2 Parameter: Das Feld, nach dem wir filtern möchten als
Lambda Expression. Der 2. Parameter ist der Wert, der gesucht werden soll. Die nachfolgende *Find()*
Methode bekommt diesen Suchfilter. Es wird allerdings noch nichts abgefragt. Erst mit *ToList()*
bzw. *FirstOrDefault()* werden die Daten in den Speicher geladen und können verwendet werden.

#### Mit AsQueryable()

Wer schon mit LINQ gearbeitet hat, findet folgenden Zugang vertrauter:

```c#
var results = db.GetCollection<Room>("rooms")
    .AsQueryable()
    .Where(r => r.Shortname == "A2.14")
    .ToList();
foreach (var result in results)
{
    Console.WriteLine($"{result.Shortname} hat eine Kapazität von {result.Capacity} Pl#tzen.");
}
```

Die Methode *AsQueryable()* liefert den Typ *IMongoQueryable* zurück, welcher die LINQ Methoden
in MongoDB Ausdrücke umwandelt. Dadurch können die gewohnten Funktionen wie *Where()*, *Select()*,
*GroupBy()*, ... verwendet werden.

## Filter in MongoDB

Die nachfolgenden Beispiele verwenden - wenn nicht anders angegeben - die Collection *teachers*.

#### Der leere Filter: {}

Möchten wir alle Dokumente einer Collection bekommen, verwenden wir den leeren Filter `{}`.
So liefern die folgenden Anweisungen in der Mongo Shell alle Documents der Collection *teachers*:

```
use examsDb
db.getCollection("teachers").find({})
```

Im MongoDB Compass geben wir einfach `{}` in die Zeile bei *Filter* ein.

#### Equals Filter { "field" : "value" }

Entspricht in SQL der Klausel *WHERE Col = Value'. Wenn wir ein Feld mit einem literal (fixen Wert)
vergleichen möchten, geben wir z. B. folgende Filter in der teachers Collection an:

- **{ "_id" : "RAU" }**   
  Sucht nach Dokumenten, wo das Feld *_id* den Wert *RAU* hat.
  Entspricht *_id = "RAU"*.
- **{ "name.email": "rau@spengergasse.at" }**   
  Wir können auch in Objekten innerhalb des Dokumentes 
  mit dem Punkt suchen. Dieser Filter sucht nach Dokumenten, wo das Feld *name.email* den Wert
  *rau@spengergasse.at* hat.
- **{ "homeOfficeDays": "MO" }**   
  *homeOfficeDays* ist ein Stringarray. Geben wir es als Feld an,
  werden alle Dokumente geliefert, die den angegebenen Wert (unter anderem) im Array haben.
- **{ "salary": null }**   
  Wir können auch nach null suchen. In Gegensatz zu SQL gibt es hier keinen
  speziellen operator.

#### Not Equals Filter { "field" : { "$ne" : "value" } }

Entspricht in SQL der Klausel *WHERE Col <> Value*. Wir können also in der teachers Collection
folgendes herausfiltern:

- **{ "hoursPerWeek" : {"$ne" : null } }**   
  Findet alle Dokumente, die einen Wert im Feld
  *hoursPerWeek* haben.
- **{ "homeOfficeDays" : { "$ne" : "MO" } }**   
  Findet alle Dokumente, die den Wert *MO* nicht in
  der Liste der homeOfficeDays haben. Dokumente mit leerer Liste werden zurückgegeben (sie haben
  ja nicht *MO* in der Liste).

#### Greater/Lower Filter { "field" : { "$gt" : "value" } } oder { "field" : { "$lt" : "value" } }

Entspricht in SQL der Klausel *WHERE Col &lt; Value*, *WHERE Col &lt;= Value*, *WHERE Col &gt; Value*,
*WHERE Col &gt;= Value*.

- **{ "hoursPerWeek": { "$gt" : 16 } }**   
  Findet alle Dokumente, die mehr als 16 Stunden im Feld
  *hoursPerWeek* gespeichert haben (hoursPerWeek > 16).
- **{ "hoursPerWeek": { "$gte" : 16 } }**   
  Findet alle Dokumente, die mehr oder gleich 16 Stunden
  im Feld *hoursPerWeek* gespeichert haben (hoursPerWeek >= 16).
- **{ "hoursPerWeek": { "$lt" : 16 } }**   
  Findet alle Dokumente, die unter 16 Stunden im Feld
  *hoursPerWeek* gespeichert haben (hoursPerWeek < 16).
- **{ "hoursPerWeek": { "$lte" : 16 } }**   
  Findet alle Dokumente, die unter oder gleich 16 Stunden
  im Feld *hoursPerWeek* gespeichert haben (hoursPerWeek <= 16).

#### Mehrere Filter mit AND verknüpfen: { "field1": Filter1, "field2": Filter2 }

Entspricht in SQL der Klausel *WHERE Col1 = Value1 AND Col2 = Value2.

- **{ "_id" : "RAU", "name.email" : "rau@spengergasse.at" }**   
  Findet alle Dokumente, wo das Feld *_id* den Wert RAU und das Feld *name.email* den Wert rau@spengergasse.at hat.
- **{ "hoursPerWeek": { "&dollar;gt" : 16 }, "salary": { "&dollar;gt" : "3000" } }**   
  Entspricht *hoursPerWeek > 16 AND salary > "3000"*. Das Gehalt (salary) ist ein decimal Wert
  und wird daher als String gespeichert. Dadurch müssen wir die Anführungszeichen setzen.

#### Mehrere Filter mit OR verknüpfen: { "$or" : [{  "field1": Filter1 }, {  "field2": Filter2 }] }

Entspricht in SQL der Klausel *WHERE Col1 = Value1 OR Col2 = Value2.


- **{ "$or" : [{ "_id" : "RAU" }, { "name.email" : "rau@spengergasse.at" }] }**   
  Entspricht der Abfrage *_id = "RAU" OR name.email = "rau@spengergasse.at"*.
- **{ "&dollar;or" : [{ "salary" : { "&dollar;gt" : "4000" } }, { "hoursPerWeek" : { "$lt" : 10 } }] }**   
  Entspricht der Abfrage *salary > 4000 OR hoursPerWeek < 10*.

#### Der all Filter { "array" : { "$all" : ["value1", "value2"] } }

Prüft, ob alle *übergebenen Werte* im gespeicherten Array vorkommen.

- **{ "homeOfficeDays" : { "$all" : ["MO", "FR"] } }**   
  Liefert alle Documents, wo das Array *homeOfficeDays* die Werte *MO* und *FR* besitzt.
  *{"MO", "DI"}* wird also *nicht* geliefert. *{"MO", "DI", "FR"}* wird zurückgeliefert.

- **{ "canTeachSubjects._id" : { "$all" : ["POS", "DBI"] } }**   
  Betrachtet für den Vergleich das Feld *_id* der Objekte im Array *canTeachSubects*. Es
  werden alle Documents geliefert, die sowohl POS als auch DBI als Wert *_id* im Array
  *canTeachSubjects* haben.

#### Filter mit regulären Ausdrücken { "field" : /RegExp/ }

Als Beispiel verwenden wir die Collection *classes*, um nach Klassen zu filtern:

- **{ "_id" : /3.AIF/ }**   
  Findet die IDs 2019W_3AAIF, 2019W_3BAIF, 2020W_3AAIF, 2020W_3BAIF, 2021W_3AAIF, 2021W_3BAIF, 2022W_3AAIF, 2022W_3BAIF
- **{ "_id" : /^2022[WS]_3.AIF$/ }**   
  Findet die IDs 2022W_3AAIF, 2022W_3BAIF

#### Der flexible where Filter: {"$where": "Expression"}

Der *where* Filter kann Javascript Ausdrücke verarbeiten. Dadurch können wir auch mehrere
Felder eines Documents miteinander vergleichen.

- **{"$where": "this._id == 'ZIP'"}**   
  Da wir das doppelte Anführungszeichen außen verwenden, verwenden wir im Ausdruck das einfache
  Anführungszeichen. Mit *this* werden die Felder des Documents angesprochen.
- **{"$where": "this.hoursPerWeek > 16"}**     
- **{"$where": "this.hoursPerWeek != null && this.salary > 500 * this.hoursPerWeek"}**   
  Wer verdient über 500 Euro pro geleisteter Wochenstunde? Wir können die aus JavaScript bekannten
  und bzw. oder Operatoren verwenden.

### Filtern mit dem MongoDB Treiber

Das nachfolgende Programm zeigt, wie diese Filterausdrücke in .NET erzeugt werden können. Es
stehen 2 Varianten zur Verfügung:

- Der Filter Builder. Hier können die Filterausdrücke aufgebaut werden. Der Builder orientiert sich
  sehr nahe an der Syntax der Filter.
- Mit *AsQueryable()*. Hier kann LINQ zur Filterung verwendet werden. Es kann allerdings nicht
  jeder Filterausdruck so generiert werden. Manchmal muss der Builder verwendet werden.

Kopiere das Generatorprogramm im Ordner *\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner
(z. B. *FilterDemos*). Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt.

```c#
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

        var examsDb = ExamDatabase.FromConnectionString("mongodb://root:1234@localhost:27017", logging: true);
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

        // Zugriff auf die darunterliegende MongbDB vom Typ IMongoDatabase.
        IMongoDatabase db = examsDb.Db;

        {
            // *************************************************************************************
            // find({ "_id" : "RAU" )
            PrintHeader("Lehrer mit der ID RAU");
            var filter = Builders<Teacher>.Filter.Eq(t => t.Id, "RAU");
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable()
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => t.Id == "RAU")
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "hoursPerWeek": { "$gt" : 16 }, "salary": { "$gt" : "3000" } })
            PrintHeader("Lehrer mit über 16 Stunden im Feld hoursPerWeek und einem Gehalt von über 3000 EUR.");
            var filter = Builders<Teacher>.Filter.And(
                Builders<Teacher>.Filter.Gt(t => t.HoursPerWeek, 16),
                Builders<Teacher>.Filter.Gt(t => t.Salary, 3000));
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable()
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => t.HoursPerWeek > 16 && t.Salary > 3000)
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "homeOfficeDays": "MO" })
            PrintHeader("Welche Lehrer haben am MO einen Home Office Tag (MO ist im Array homeOfficeDays enthalten)?");
            var filter = Builders<Teacher>.Filter.AnyEq(t => t.HomeOfficeDays, "MO");
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable()
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => t.HomeOfficeDays.Any(h => h == "MO"))
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "homeOfficeDays" : { "$all" : ["MO", "FR"] } })
            PrintHeader("Welche Lehrer haben am MO und am FR einen Home Office Tag (MO UND FR ist im Array homeOfficeDays enthalten)?");
            var filter = Builders<Teacher>.Filter.All(t => t.HomeOfficeDays, new string[] { "MO", "FR" });
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable()
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => t.HomeOfficeDays.Any(h => h == "MO") && t.HomeOfficeDays.Any(h => h == "FR"))
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "canTeachSubjects._id" : { "$all" : ["POS", "DBI"] } })
            PrintHeader("Welche Lehrer können POS und DBI unterrichten (POS UND DBI ist im Array canTeachSubjects als Id enthalten)?");
            // Wir geben das Feld als String an, da canTeachSubjects ein Array ist.
            // Beachte, dass das Feld Shortname in der Klasse Subject intern als _id gespeichert wird.
            var filter = Builders<Teacher>.Filter.All("canTeachSubjects._id", new string[] { "POS", "DBI" });
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable()
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => 
                    t.CanTeachSubjects.Any(s => s.Shortname == "POS") &&
                    t.CanTeachSubjects.Any(s => s.Shortname == "DBI"))
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "_id" : /3.AIF/ })
            PrintHeader("Alle 3. Semester AIF Klassen in der Collection classes.");
            var filter = Builders<Class>.Filter.Regex(c=>c.Id, @"3.AIF");
            var results = db.GetCollection<Class>("classes").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "_id" : /^2022[WS]_3.AIF$/ })
            PrintHeader("Alle 3. Semester AIF Klassen des Schuljahres 2022 in der Collection classes.");
            var filter = Builders<Class>.Filter.Regex(c => c.Id, @"^2022[WS]_3.AIF$");
            var results = db.GetCollection<Class>("classes").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
        }
        {
            // *************************************************************************************
            // find({ "$where" : "this.hoursPerWeek != null && this.salary > 500 * this.hoursPerWeek" })
            PrintHeader("Lehrer, die mehr als 500 Euro pro Wochenstunde bekommen (Gehalt > 500 * hoursPerWeek).");
            // Achte auf die Anführungszeichen! @ bedeutet verbatim string (kein Escapen von Sonderzeichen).
            // Im Inneren müssen die Anführungszeichen doppelt geschrieben werden (" --> "")
            var results = db.GetCollection<Teacher>("teachers").Find(@"{$where: ""this.hoursPerWeek != null && this.salary > 500 * this.hoursPerWeek""}").ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
        }
        return 0;
    }
    static void PrintHeader(string text)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(Environment.NewLine + text);
        Console.ForegroundColor = color;
    }
}
```

## Übung

Du kannst die folgende Aufgabe auf 3 Arten lösen:

1. Schreiben der Filter in MongoDb Compass
2. Verwenden des FilterBuilders in .NET
3. Verwenden der Methode *AsQueryable()* (wenn möglich).

Die Aufgaben sind im untenstehenden Programmcode als Kommentar. Falls du die Aufgabe im
Programmcode lösen möchtest, gehe so vor: Kopiere das Generatorprogramm im Ordner
*\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner (z. B. *FilterExcercise*).
Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt und schreibe das Ergebnis
deiner Abfrage in die Variable *result*. Die korrekte Ausgabe ist unten angeführt.


```c#
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

        // HINWEIS FÜR DEN ZUGRIFF AUF COLLECTIONS
        // In der Klasse ExamDatabase ist ein Property für jede Collection definiert. So kann mit
        // examsDb.Classes auf classes zugegriffen werden. Damit erspart man sich das Schreiben von examsDb.Db.GetCollection<Class>("classes").
        //
        // examsDb.Terms    entspricht examsDb.Db.GetCollection<Term>("terms")
        // examsDb.Subjects entspricht examsDb.Db.GetCollection<Subject>("subjects")
        // examsDb.Rooms    entspricht examsDb.Db.GetCollection<Room>("rooms")
        // examsDb.Classes  entspricht examsDb.Db.GetCollection<Class>("classes")
        // examsDb.Students entspricht examsDb.Db.GetCollection<Student>("students")
        // examsDb.Teachers entspricht examsDb.Db.GetCollection<Teacher>("teacjers")
        // examsDb.Exams    entspricht examsDb.Db.GetCollection<Exam>("exams")

        // Muster: Alle weiblichen Studierenden, die gerade in der 5AAIF sind (CurrentClass):
        // Variante 1: Mit dem Filter Builder
        {
            PrintHeader("MUSTER: Weibliche Studierende der 6AAIF (mit dem Builder).");
            var filter = Builders<Student>.Filter.And(
                Builders<Student>.Filter.Eq(s => s.Gender, Gender.Female),
                Builders<Student>.Filter.Eq(s => s.CurrentClass!.Shortname, "6AAIF")
            );
            IEnumerable<Student> result = examsDb.Students.Find(filter).ToList();
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} ist gerade in der 6AAIF."));
        }
        // Variante 2: Mit AsQueryable()
        {
            PrintHeader("MUSTER: Weibliche Studierende der 6AAIF (mit AsQueryable()).");
            IEnumerable<Student> result = examsDb.Students.AsQueryable().Where(s => s.CurrentClass!.Shortname == "6AAIF" && s.Gender == Gender.Female);
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} ist gerade in der 6AAIF."));
        }
        // *****************************************************************************************
        // (1) Welche Klassen gibt es in der Classes Collection im Schuljahr 2021 (Year ist 2021)
        //     der Abteilung CIF?
        {
            PrintHeader("Klassen im Jahr 2021 der CIF Abteilung.");
            IEnumerable<Class> result = Enumerable.Empty<Class>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Shortname} mit KV {r.ClassTeacher.Shortname}"));
        }

        // *****************************************************************************************
        // (2) Welche Räume haben eine Kapazität von 30 oder mehr Plätzen (Capacity)?
        {
            PrintHeader("Räume über 30 Sitzplätze.");
            IEnumerable<Room> result = Enumerable.Empty<Room>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Shortname).ToList().ForEach(r => Console.WriteLine($"{r.Shortname}: {r.Shortname} hat {r.Capacity} Plätze."));
        }

        // *****************************************************************************************
        // (3) Gib alle Klassen ab dem 5. Semester der KIF Abteilung im Jahr 2022 aus (EducationLevel >= 5).
        {
            PrintHeader("Klassen ab dem 5. Semester der KIF im Jahr 2022.");
            IEnumerable<Class> result = Enumerable.Empty<Class>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Shortname} mit KV {r.ClassTeacher.Shortname}"));
        }

        // *****************************************************************************************
        // (4) Welche Studierenden sind vor dem 1.1.1991 geboren?
        {
            PrintHeader("Studierende, die vor dem 1.1.1991 geboren sind.");
            IEnumerable<Student> result = Enumerable.Empty<Student>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} ist am {r.DateOfBirth:dd.MM.yyyy} geboren."));
        }


        // *****************************************************************************************
        // (5) Welche Klassen des Wintersemesters 2022 haben keinen Stammraum (RoomShortname ist null)?
        {
            PrintHeader("Klassen ohne Stammraum im Wintersemester 2022/23.");
            IEnumerable<Class> result = Enumerable.Empty<Class>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Shortname} mit KV {r.ClassTeacher.Shortname}"));
        }

        // *****************************************************************************************
        // (6) Welche negativen Prüfungen gab es zwischen 1.1.2022 und 27.1.2022?
        // Erstelle mit new DateTime(Year, Month, Day) einen Datums/Zeitwert. Beachte, dass das Endedatum als "kleiner" Filter und mit 28.1.2022
        // gesetzt werden muss. Die erstellten DateTime Werte haben nämlich als Zeitwert 0:00, sonst werden die Prüfungen am letzten
        // Tag nicht gelistet!
        {
            PrintHeader("Negative Prüfungen zwischen 1.1.2022 und inkl. 27.1.2022.");
            IEnumerable<Exam> result = Enumerable.Empty<Exam>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: Exam von {r.Student.Firstname} {r.Student.Lastname} am {r.DateTime} in {r.Subject.Shortname} bei {r.Teacher.Shortname} mit Note {r.Grade}"));
        }

        // *****************************************************************************************
        // (7) Welche Lehrer dürfen das Fach POS unterrichten (haben also POS in der Liste CanTeachSubjects)?
        {
            PrintHeader("Lehrer, die POS unterrichten können.");
            IEnumerable<Teacher> result = Enumerable.Empty<Teacher>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} darf POS unterrichten."));
        }


        // *****************************************************************************************
        // (8) Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF besucht?
        {
            PrintHeader("Studierende, die 2021/22 die 3BKIF besucht haben.");
            IEnumerable<Student> result = Enumerable.Empty<Student>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} war 2021/22 in der 3BKIF."));
        }

        // *****************************************************************************************
        // (9) Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF,
        //     aber 2022/23 nicht die 5BKIF besucht?
        {
            PrintHeader("Studierende, die 2021/22 die 3BKIF, aber  2022/23 nicht die 5BKIF besucht haben.");
            IEnumerable<Student> result = Enumerable.Empty<Student>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: {r.Name.Firstname} {r.Name.Lastname} war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF."));
        }

        // (10) Welche Prüfungen erreichten maximal 25% der Punkte (Points ist also <= PointsMax * 0.25)?
        {
            PrintHeader("Prüfungen <= 25%");
            IEnumerable<Exam> result = Enumerable.Empty<Exam>(); // TODO: Schreibe das Ergebnis deiner Abfrage in result.
            result.OrderBy(r => r.Id).ToList().ForEach(r => Console.WriteLine($"{r.Id}: Die Prüfung von {r.Student.Firstname} {r.Student.Lastname} in {r.Subject.Shortname} hat nur {r.Points} von {r.PointsMax} Punkte."));
        }
        return 0;
    }
    static void PrintHeader(string text)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(Environment.NewLine + text);
        Console.ForegroundColor = color;
    }
}
```

**Korrekte Ausgabe**
```
Klassen im Jahr 2021 der CIF Abteilung.
2021S_4ACIF: 4ACIF mit KV GIL
2021S_4BCIF: 4BCIF mit KV BRI
2021S_6ACIF: 6ACIF mit KV BUS
2021S_6BCIF: 6BCIF mit KV HER
2021S_8ACIF: 8ACIF mit KV CAM
2021S_8BCIF: 8BCIF mit KV MÖR
2021W_3ACIF: 3ACIF mit KV GIL
2021W_3BCIF: 3BCIF mit KV BRI
2021W_5ACIF: 5ACIF mit KV BUS
2021W_5BCIF: 5BCIF mit KV HER
2021W_7ACIF: 7ACIF mit KV CAM
2021W_7BCIF: 7BCIF mit KV MÖR

Räume über 30 Sitzplätze.
AH.14: AH.14 hat 32 Plätze.
B2.10: B2.10 hat 30 Plätze.

Klassen ab dem 5. Semester der KIF im Jahr 2022.
2022S_6AKIF: 6AKIF mit KV SAC
2022S_6BKIF: 6BKIF mit KV HER
2022W_5AKIF: 5AKIF mit KV SAC
2022W_5BKIF: 5BKIF mit KV HER

Studierende, die vor dem 1.1.1991 geboren sind.
100072: Jolie Reiber ist am 04.09.1990 geboren.
100075: Artur Winkler ist am 19.09.1990 geboren.
100266: Torben Scheuring ist am 19.11.1990 geboren.
100271: Konrad Damaske ist am 13.12.1990 geboren.
100274: Tarja Grundmann ist am 05.11.1990 geboren.
100281: Clara Plautz ist am 19.11.1990 geboren.
100459: Steve Möhsner ist am 05.09.1990 geboren.
100470: Dominic Hansen ist am 21.12.1990 geboren.
100476: Maxi Blockhaus ist am 02.09.1990 geboren.

Klassen ohne Stammraum im Wintersemester 2022/23.
2022W_3AAIF: 3AAIF mit KV KUR
2022W_3ACIF: 3ACIF mit KV BRI
2022W_3BKIF: 3BKIF mit KV KNE
2022W_5ABIF: 5ABIF mit KV MÖR
2022W_7ABIF: 7ABIF mit KV CAM

Negative Prüfungen zwischen 1.1.2022 und inkl. 27.1.2022.
634c4dacdd058d6535c4e340: Exam von Lillian Schirrmeister am 09/01/2022 17:00:00 in D bei ERH mit Note 5
634c4dacdd058d6535c4e39b: Exam von Silas Schlicht am 26/01/2022 14:00:00 in D bei SCH mit Note 5
634c4dacdd058d6535c4e3df: Exam von Corinna Menne am 21/01/2022 14:00:00 in AM bei KUR mit Note 5
634c4dacdd058d6535c4e405: Exam von Marian Krämer am 03/01/2022 08:00:00 in AM bei MÖR mit Note 5
634c4dacdd058d6535c4e452: Exam von Leonidas Strieder am 16/01/2022 12:00:00 in DBI bei KUR mit Note 5
634c4dacdd058d6535c4e491: Exam von Sönke Sonn am 06/01/2022 09:00:00 in D bei ERH mit Note 5

Lehrer, die POS unterrichten können.
CAM: Stefanie Camara darf POS unterrichten.
DAU: Jannik Dauer darf POS unterrichten.
HAR: Vito Harting darf POS unterrichten.
KNE: Timm Kneifel darf POS unterrichten.
KUR: Jennifer Kurschilgen darf POS unterrichten.
NOR: Arda Norris darf POS unterrichten.
SCH: Caroline Schondelmaier darf POS unterrichten.
THR: Dion Thriene darf POS unterrichten.

Studierende, die 2021/22 die 3BKIF besucht haben.
100421: Jana Schnieder war 2021/22 in der 3BKIF.
100422: Maira Grube war 2021/22 in der 3BKIF.
100423: Fatima Rose war 2021/22 in der 3BKIF.
100424: Danny Stern war 2021/22 in der 3BKIF.
100425: Jano Welzel war 2021/22 in der 3BKIF.
100426: Lewis Grau war 2021/22 in der 3BKIF.
100427: Hanno Lichtl war 2021/22 in der 3BKIF.
100428: Loreen Östringer war 2021/22 in der 3BKIF.
100429: Henrick Renz war 2021/22 in der 3BKIF.
100430: Jean Hördt war 2021/22 in der 3BKIF.
100431: Ellen Hort war 2021/22 in der 3BKIF.
100432: Nevio Gruber war 2021/22 in der 3BKIF.
100433: Selma Dauer war 2021/22 in der 3BKIF.
100434: Sean Kolokas war 2021/22 in der 3BKIF.
100435: Ahmet Sladek war 2021/22 in der 3BKIF.
100436: Rayan Urban war 2021/22 in der 3BKIF.
100437: Lucienne Gelling war 2021/22 in der 3BKIF.
100438: Xaver Osenberg war 2021/22 in der 3BKIF.
100439: Malte Bartels war 2021/22 in der 3BKIF.
100440: Annabel Heinke war 2021/22 in der 3BKIF.
100441: Greta Marahrens war 2021/22 in der 3BKIF.
100442: Marc Beckmann war 2021/22 in der 3BKIF.
100443: Leopold Gabius war 2021/22 in der 3BKIF.
100444: Sammy Kúhn war 2021/22 in der 3BKIF.
100445: Riana Schmitz war 2021/22 in der 3BKIF.
100446: Asya Beckmann war 2021/22 in der 3BKIF.
100447: Julius Lesch war 2021/22 in der 3BKIF.

Studierende, die 2021/22 die 3BKIF, aber nicht 2022/23 die 5BKIF besucht haben.
100427: Hanno Lichtl war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.
100429: Henrick Renz war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.
100430: Jean Hördt war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.
100431: Ellen Hort war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.
100438: Xaver Osenberg war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.
100443: Leopold Gabius war 2021/22 in der 3BKIF, aber 2022/23 nicht in der 5BKIF.

Prüfungen <= 25%
634c4dacdd058d6535c4e323: Die Prüfung von Efe Sander in D hat nur 6 von 27 Punkte.
634c4dacdd058d6535c4e336: Die Prüfung von Melanie Balnuweit in POS hat nur 8 von 33 Punkte.
634c4dacdd058d6535c4e33b: Die Prüfung von Ina Sander in POS hat nur 11 von 44 Punkte.
634c4dacdd058d6535c4e347: Die Prüfung von Melek Fassbender in AM hat nur 8 von 35 Punkte.
634c4dacdd058d6535c4e34a: Die Prüfung von Eliah Schnürer in DBI hat nur 6 von 24 Punkte.
634c4dacdd058d6535c4e376: Die Prüfung von Justine Tsamonikian in D hat nur 5 von 20 Punkte.
634c4dacdd058d6535c4e396: Die Prüfung von Mariella Marchewski in D hat nur 5 von 22 Punkte.
634c4dacdd058d6535c4e39a: Die Prüfung von Corinna Menne in POS hat nur 7 von 31 Punkte.
634c4dacdd058d6535c4e39f: Die Prüfung von Viktoria Marschek in DBI hat nur 9 von 39 Punkte.
634c4dacdd058d6535c4e3ae: Die Prüfung von Lutz Frank in D hat nur 8 von 32 Punkte.
634c4dacdd058d6535c4e3b8: Die Prüfung von Ramon Pingpank in AM hat nur 5 von 21 Punkte.
634c4dacdd058d6535c4e3ba: Die Prüfung von Julius Lesch in DBI hat nur 8 von 35 Punkte.
634c4dacdd058d6535c4e3bf: Die Prüfung von Patrice Vogelgsang in AM hat nur 10 von 40 Punkte.
634c4dacdd058d6535c4e3dc: Die Prüfung von Amina Effler in D hat nur 4 von 16 Punkte.
634c4dacdd058d6535c4e3e4: Die Prüfung von Giada Schorr in AM hat nur 4 von 17 Punkte.
634c4dacdd058d6535c4e3f2: Die Prüfung von Josua Klein in POS hat nur 10 von 41 Punkte.
634c4dacdd058d6535c4e41b: Die Prüfung von Lennart Kupprion in DBI hat nur 6 von 26 Punkte.
634c4dacdd058d6535c4e41c: Die Prüfung von Liv Maczey in DBI hat nur 5 von 21 Punkte.
634c4dacdd058d6535c4e42d: Die Prüfung von Ahmet Krug in D hat nur 10 von 43 Punkte.
634c4dacdd058d6535c4e472: Die Prüfung von Oskar Huth in DBI hat nur 12 von 48 Punkte.
```

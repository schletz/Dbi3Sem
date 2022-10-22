# Update von Dokumenten

## Absetzen von Anfragen in der Shell

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

## Das updateOne und updateMany Kommando

Das *updateOne* und *updateMany* Kommando hat den gleichen Aufbau. Der erste Parameter ist ein Filter,
der angibt, welche Dokumente aktualisiert werden sollen. Das ist vergleichbar mit dem
WHERE in einer SQL UPDATE Anweisung. Als zweiten Parameter werden die neuen Werte übergeben.


> ### db.getCollection(name).updateOne(filter, update, options)
> ### db.getCollection(name).updateMany(filter, update, options)

Die genaue Beschreibung der Kommandos ist in der MongoDB Doku auf
https://www.mongodb.com/docs/manual/reference/method/db.collection.updateOne/ bzw.
https://www.mongodb.com/docs/manual/reference/method/db.collection.updateMany/ verfügbar.


## Einige Beispiele für Updates

Den Aufbau des Filters haben wir schon im letzten Kapitel (Find) besprochen. Das Wissen über
Filter ist in diesem Kapitel notwendig.

### Updates, die Werte setzen

Das Feld hoursPerWeek von Lehrer HAR wird auf 18 Stunden gesetzt. Wir suchen zuerst nach dem
Lehrer mit der ID *HAR* und setzen dann mit dem *$set* Operator den Wert auf 18. Da wir nach
dem Key filtern, genügt ein *updateOne()*, da ohnehin nur 1 Datensatz gefunden werden kann.

```
db.getCollection("teachers").updateOne(
    { "_id" : "HAR" },
    { "$set" : { "hoursPerWeek" : 18 } })
```

Alle Lehrenden, die hoursPerWeek < 10 haben, werden auf hoursPerWeek = 10 gesetzt. Da mehrere
Datensätze betroffen sein könnten, verwenden wir *updateMany()*.

```
db.getCollection("teachers").updateMany(
    { "hoursPerWeek" : { "$lt" : 10 } },
    { "$set" : { "hoursPerWeek" : 10 } })
```

Die Klasse mit der ID *2022W_3AAIF* bekommt CAM als neuen Klassenvorstand. In einem Programm muss
mit dem Suchfilter `{ "_id" : "CAM" }` der Datensatz des Lehrers *CAM* gelesen werden. Dann können
wir *nur die Infos des Objektes name* auslesen und diese dem Feld *classTeacher* zuweisen:

```
db.getCollection("classes").updateOne(
    { "_id" : "2022W_3AAIF" },
    { "$set" : { "classTeacher" : { "shortname" : "CAM", "firstname" : "Stefanie", "lastname" : "Camara", "email" : "camara@spengergasse.at" } } })
```

### Updates, die Arrays in Dokumenten ändern

Lehrer ROS bekommt den DI als Home Office Tag. Der *$push* Operator fügt Werte zu einem Array hinzu:

```
db.getCollection("teachers").updateOne(
    { "_id" : "ROS" },
    { "$push" : { "homeOfficeDays" : "DI" } })
```

Lehrerin SAC bekommt den DI als Home Office Tag. Das Problem: Sie hat bereits den DI im Array
*homeOfficeDays*. Mit *$push* würden wir also einen doppelten Eintrag produzieren. Der
Operator *$addToSet* prüft, ob der Wert nicht schon im Array vorhanden ist:

```
db.getCollection("teachers").updateOne(
    { "_id" : "SAC" },
    { "$addToSet" : { "homeOfficeDays" : "DI" } })
```

Lehrerin ZIP bekommt MI und FR als Home Office Tag. Den Operator *$addToSet* können wir auch mit
*$each* verbinden. So wird MI und FR zum Array hinzugefügt, wenn diese Werte nicht schon
vorhanden sind:

```
db.getCollection("teachers").updateOne(
    { "_id" : "ZIP" }
    { "$addToSet" : { "homeOfficeDays" : { "$each" : ["MI", "FR"] } } })
```

Der FR wird im Array homeOfficeDays aller Lehrenden gelöscht. Dafür gibt es den *$pull*
Operator, um Elemente aus einem Array zu entfernen. Der Suchfilter ist leer, somit werden
alle Dokumente bearbeitet.

```
db.getCollection("teachers").updateMany(
    { },
    { "$pull" : { "homeOfficeDays" : "FR" } })
```

Der MO wird im Array *homeOfficeDays* aller Lehrenden auf FR geändert. Dafür verwenden wir den
*positional $ operator* (siehe https://www.mongodb.com/docs/manual/reference/operator/update/positional/#up._S)
Wird er in Verbindung mit einer Filteranweisung verwendet, liefert er alle Werte im Array, die dem
Filter entsprechen. Die restlichen Werte im Array bleiben unverändert.

```
db.getCollection("teachers").updateMany(
    { "homeOfficeDays" : "MO" },
    { "$set" : { "homeOfficeDays.$" : "FR" } })
```

### Addieren und Multiplizieren

Lehrermangel: Alle Lehrenden, die DBI unterrichten können und unter 20 Wochenstunden unterrichten,
bekommen 5 Stunden dazu. In SQL würden wir *SET hoursPerWeek = hoursPerWeek + 5* schreiben. In
MongoDB gibt es einen eigenen *$inc* Operator. Hinweis: Dekrement (verringern) ist ein *$inc* mit
negativem Vorzeichen.

```
db.getCollection("teachers").updateMany(
    { "canTeachSubjects._id" : "DBI", "hoursPerWeek" : { "$lt" : 20 } },
    { "$inc" : { "hoursPerWeek" : 5 } })
```

Einsparung bei Lehrern: Alle Lehrenden, die mehr als 4000 Euro verdienen, bekommen nur mehr 90% ihres
Gehalts. Hier verwenden wir den *$mul* Operator, um ein Feld mit 0.9 zu multiplizieren.

```
db.getCollection("teachers").updateMany(
    { "salary" : { "$gt" : 4000 } },
    { "$mul" : { "salary" : 0.9 } })

```

### Update Anomalie

Lehrerin BRI bekommt die Mailadresse bri@spengergasse.at. Dies ist eine einfache *$set* Operation
```
db.getCollection("teachers").updateOne(
    { "_id" : "BRI" },
    { "$set" : { "name.email" : "bri@spengergasse.at" } })
```

Nun haben wir allerdings folgendes zu berücksichtigen. In den Dokumenten der Collection *classes*
wird *shortname*, *firstname* , *lastname* und *email* im Feld *classTeacher* gespeichert.
In unserem Fall ist es sinnvoll, die Dokumente des aktuellen Schuljahres (*term.year* ist 2022)
zu aktualisieren. Die älteren Klassen lassen wir unverändert, da in den vergangenen Jahren der
Lehrer diese E-Mail Adresse hatte.

Natürlich kann das auch anders gehandhabt werden (aktualisieren aller Klassen). Es muss sich also
vorab überlegt werden, ob Aktualisierungen in den eingebetteten Dokumenten vorgenommen werden. Im
Programmcode macht dies dann das *Repository Pattern*, das bescheid weiß welche Dokumente mit aktualisiert
werden müssen.

```
db.getCollection("classes").updateMany({ "term.year" : 2022, "classTeacher.shortname" : "BRI" }, { "$set" : { "classTeacher.email" : "bri@spengergasse.at" } })
```

### Ausblick: Pipelines

Lehrende mit über 4000 EUR Gehalt bekommen das Gehalt nach der Formel 200 x hoursPerWeek. Würden
wir einfach *hoursPerWeek* mit dem *$mul* Operator mit 200 multiplizieren, bedeutet dies dass
der neue Wert von *hoursPerWeek* der alte Wert von *hoursPerWeek* x 200 ist. *$mul* ist also
wie in Java oder C# der Operator `*=`.

```
db.getCollection("teachers").updateMany(
    {"hoursPerWeek": {"$ne": null}, "salary": { "$gt": 3000 } },
    { "$mul": { "hoursPerWeek": 200 } })
```

Die Lösung bietet eine sogenannte Pipeline. Sie ist daran erkennbar dass im Update Parameter
ein Array verwendet wird.

```
db.getCollection("teachers").updateMany(
    { "salary" : { "$gt" : NumberDecimal("4000") } },
    [{ "$set" : { "salary" : { "$multiply" : ["$hoursPerWeek", NumberDecimal("200")] } } }])
```

## Updates mit dem .NET Treiber von MongoDB

Kopiere das Programm im Ordner *13_NoSQL\ExamsDb* in einen eigenen Ordner und ersetze die Datei
*Program.cs* durch den folgenden Inhalt. In der Konsole werden die Update Statements ausgegeben:

```c#
using ExamDbGenerator;
using MongoDB.Driver;
using System;
using ExamDbGenerator.Model;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;

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

        var db = examsDb.Db;

        // *****************************************************************************************
        // Lehrer HAR möchte 18 Stunden unterrichten (statt der eingetragenen 14). Es muss das Feld
        // hoursPerWeek also auf 18 gesetzt werden.
        // Da wir nach der ID filtern, bekommen wir nur 1 Datensatz. Daher können wir udpateOne
        // verwenden.
        {
            PrintHeader("Das Feld hoursPerWeek von Lehrer HAR wird auf 18 Stunden gesetzt.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateOne(
                    Builders<Teacher>.Filter.Eq(t => t.Id, "HAR"),
                    Builders<Teacher>.Update.Set(t => t.HoursPerWeek, 18));
            Console.WriteLine($"{result.MatchedCount} Datensätze in teachers gefunden.");
        }


        // *****************************************************************************************
        // Alle Lehrenden, die unter 10 Stunden unterrichten wollen (hoursPerWeek < 10), sollen jetzt
        // 10 Stunden unterrichten.
        // Wir brauchen UpdateMany, da wir mehrere Datensätze bekommen (können).
        {
            PrintHeader("Alle Lehrenden, die hoursPerWeek < 10 haben, werden auf hoursPerWeek = 10 gesetzt.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.Lt(t => t.HoursPerWeek, 10),
                    Builders<Teacher>.Update.Set(t => t.HoursPerWeek, 10));
            Console.WriteLine($"{result.MatchedCount} Datensätze in teachers gefunden.");
        }

        // *****************************************************************************************
        // Die Klasse 2022W_3AAIF bekommt einen neuen Klassenvorstand: CAM.
        // Wir müssen das name Objekt des entsprechenden Teachers Document zuweisen.
        {
            PrintHeader("Die Klasse mit der ID 2022W_3AAIF bekommt CAM als neuen Klassenvorstand.");
            var teacherName = db.GetCollection<Teacher>("teachers")
            .Find(Builders<Teacher>.Filter.Eq(t => t.Id, "CAM"))
            .First()  // Exception wenn kein Teacher gefunden wurde. FirstOrDefault(), um in diesem Fall null zu bekommen.
            .Name;    // Nur das Name Property lesen. Sonst wird das ganze Teacher objekt eingefügt!
            var result = db.GetCollection<Class>("classes")
                .UpdateOne(
                    Builders<Class>.Filter.Eq(c => c.Id, "2022W_3AAIF"),
                    Builders<Class>.Update.Set(c => c.ClassTeacher, teacherName)
                );
        }

        // *************************************************************************************************
        // Lehrer ROS bekommt den DI als HomeOfficeTag.
        // Wir verwenden Push. Dies hängt einfach einen Wert im Array an.
        {
            PrintHeader("Lehrer ROS bekommt den DI als Home Office Tag (mit Push)");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateOne(
                    Builders<Teacher>.Filter.Eq(t => t.Id, "ROS"),
                    Builders<Teacher>.Update.Push(t => t.HomeOfficeDays, "DI")
                );
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *************************************************************************************************
        // Lehrerin SAC bekommt auch den DI als HomeOfficeTag. Sie hat aber schon den DI eingetragn.
        // Mit Push würden wir 2x den Dienstag
        // haben. Wir verwenden AddToSet, um keine Duplikate zu erzeugen.
        {
            PrintHeader("Lehrerin SAC bekommt den DI als Home Office Tag (mit AddToSet)");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateOne(
                    Builders<Teacher>.Filter.Eq(t => t.Id, "SAC"),
                    Builders<Teacher>.Update.AddToSet(t => t.HomeOfficeDays, "DI")
                );
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *************************************************************************************************
        // Lehrerin ZIP bekommt MI und FR HomeOfficeTag. 
        // Wir verwenden AddToSet, um keine Duplikate zu erzeugen.
        {
            PrintHeader("Lehrerin ZIP bekommt MI und FR als Home Office Tag (mit AddToSetEach)");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateOne(
                    Builders<Teacher>.Filter.Eq(t => t.Id, "ZIP"),
                    Builders<Teacher>.Update.AddToSetEach(t => t.HomeOfficeDays, new string[] { "MI", "FR" })
                );
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *************************************************************************************************
        // Überstunden wegen Lehrermangels: Alle Lehrer, die in DBI unterrichten können (canTeachSubjects)
        // 20 Wochenstunden unterrichten, bekommen 5 Stunden dazu.
        {
            PrintHeader("Alle Lehrenden, die DBI unterrichten können und unter 20 Wochenstunden unterrichten, bekommen 5 Stunden dazu.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.And(
                        Builders<Teacher>.Filter.AnyEq("canTeachSubjects._id", "DBI"),
                        Builders<Teacher>.Filter.Lt(t => t.HoursPerWeek, 20)),
                    Builders<Teacher>.Update.Inc(t => t.HoursPerWeek, 5));
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *****************************************************************************************
        // Einsparung: Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen nur mehr 90% ihres Gehalts.
        {
            PrintHeader("Alle Lehrenden, die mehr als 4000 Euro verdienen, bekommen nur mehr 90% ihres Gehalts.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.Gt(t => t.Salary, 4000),
                    Builders<Teacher>.Update.Mul(t => t.Salary, 0.9M));
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }


        // *****************************************************************************************
        // Der FR als Home Office Tag wird gestrichen. Er ist bei allen Lehrenden zu löschen.
        {
            PrintHeader("Der FR wird im Array homeOfficeDays aller Lehrenden gelöscht.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.Empty,
                    Builders<Teacher>.Update.Pull(t => t.HomeOfficeDays, "FR"));
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *****************************************************************************************
        // Der MO als Home Office Tag wird für alle Lehrenden auf FR verschoben.
        // Wir verwenden den positional $ operator (siehe https://www.mongodb.com/docs/manual/reference/operator/update/positional/#up._S_)
        // Da nach dem Montag gefiltert wird, wird auch nur der MO auf FR geändert.
        {
            PrintHeader("Der MO wird im Array homeOfficeDays aller Lehrenden auf FR geändert.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.AnyEq(t => t.HomeOfficeDays, "MO"),
                    Builders<Teacher>.Update.Set("homeOfficeDays.$", "FR"));
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
        }

        // *****************************************************************************************
        // Lehrerin BRI ändert die Mailadresse auf bri@spengergasse.at (wegen zu viel Spam).
        // Wir aktualisieren daher einmal das Document in der teachers Collection.
        // updateOne genügt, da wir nach der ID filtern.
        {
            PrintHeader("Lehrerin BRI bekommt die Mailadresse bri@spengergasse.at.");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateOne(
                    Builders<Teacher>.Filter.Eq(t => t.Id, "BRI"),
                    Builders<Teacher>.Update.Set(t => t.Name.Email, "bri@spengergasse.at"));
            Console.WriteLine($"{result.MatchedCount} Datensätze in teachers gefunden.");
        }
        // Lehrerin BRI ist auch classTeacher in den Documents der classes Collection. Was soll
        // also getan werden ("Update Anomalie")? Wir aktualisieren alle Klassen des heurigen
        // Schuljahres.
        // Dafür brauchen wir updateMany, da BRI von mehreren Klassen Klassenvorständin sein kann.
        {
            var result = db.GetCollection<Class>("classes")
                .UpdateMany(
                    Builders<Class>.Filter.And(
                        Builders<Class>.Filter.Eq(c => c.Term.Year, 2022),
                        Builders<Class>.Filter.Eq(c => c.ClassTeacher.Shortname, "BRI")),
                    Builders<Class>.Update.Set(c => c.ClassTeacher.Email, "bri@spengergasse.at"));
            Console.WriteLine($"{result.MatchedCount} Datensätze in classes gefunden.");
        }
        // Was ist mit ihrem Eintrag in den Documents der exams Collection? Soll dieser
        // auch aktualisiert werden? Das hängt vom Sachverhalt ab.

        // *****************************************************************************************
        // Ausblick auf Pipelines: Lehrende mit über 4000 EUR Gehalt bekommen das Gehalt nach der
        // Formel 200 * hoursPerWeek.
        {
            PrintHeader("Lehrende mit über 4000 EUR Gehalt bekommen das Gehalt nach der Formel 200 * hoursPerWeek .");
            var result = db.GetCollection<Teacher>("teachers")
                .UpdateMany(
                    Builders<Teacher>.Filter.Gt(t => t.Salary, 4000),
                    new EmptyPipelineDefinition<Teacher>()
                        .AppendStage<Teacher, Teacher, Teacher>(@"{ '$set': { 'salary' : { '$multiply': [ '$hoursPerWeek', NumberDecimal(200) ] } } }"));
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
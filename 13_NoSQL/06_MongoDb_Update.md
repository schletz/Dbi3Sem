# Update von Dokumenten

## Absetzen von Befehlen in der Shell von Studio 3T

Nachdem du dich mit der Datenbank *examsDb* in Studio 3T verbunden hast, kannst du die unten
beschriebenen Befehle absetzen.

![](studio3t_shell_2122.png)

## Die updateOne und updateMany Funktion (mongoshell)

Die *updateOne* und *updateMany* Funktion hat den gleichen Aufbau. Der erste Parameter ist ein Filter,
der angibt, welche Dokumente aktualisiert werden sollen. Das ist vergleichbar mit dem
WHERE in einer SQL UPDATE Anweisung. Als zweiten Parameter werden die neuen Werte übergeben.


> ### db.getCollection(name).updateOne(filter, update, [options])
> ### db.getCollection(name).updateMany(filter, update, [options])

Wie der Name schon sagt aktualisiert *updateOne()* nur ein Dokument, während *updateMany()*
alle Dokumente aktualisiert die dem Suchfilter entsprechen. Bei Filterungen nach der ID
verwenden wir daher *updateOne()*, ansonsten *updateMany()*.
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

Alle Lehrenden, die *hoursPerWeek* < 10 haben, werden auf *hoursPerWeek* = 10 gesetzt. Da mehrere
Datensätze betroffen sein könnten, verwenden wir *updateMany()*.

```
db.getCollection("teachers").updateMany(
    { "hoursPerWeek" : { "$lt" : 10 } },
    { "$set" : { "hoursPerWeek" : 10 } })
```

Start- und Endedatum des Semesters *2023W* wird auf den 4.9.2023 bzw. auf den 5.2.2024 gesetzt. Wir
können auch mehrere Felder mit dem *$set* Operator auf einmal setzen.

```
db.getCollection("terms").updateOne(
    { "_id" : "2023W" },
    { "$set" : { "start" : "2023-09-04", "end" : "2024-02-05" } })
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
db.getCollection("classes").updateMany(
    { "term.year" : 2022, "classTeacher.shortname" : "BRI" },
    { "$set" : { "classTeacher.email" : "bri@spengergasse.at" } })
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
    [{ "$addFields" : { "salary" : { "$multiply" : ["$hoursPerWeek", NumberDecimal("200")] } } }])
```

## ReplaceOne

Es gibt auch eine Anweisung, um ein Dokument zur Gänze durch neue Daten zu ersetzen.

> ### db.getCollection(name).replaceOne(filter, new_document, options)

Beispiel: 

```
db.getCollection("teachers").updateOne(
    { "_id" : "HAR" }, 
    { 
        "_id" : "HAR", "name" : { "shortname" : "HAR", "firstname" : "Vito", "lastname" : "Harting", "email" : "harting@spengergasse.at" },
        "gender" : "Male", "hoursPerWeek" : 14, "lessonsFrom" : "16:00", "salary" : NumberDecimal("1000"), "homeOfficeDays" : [], 
        "canTeachSubjects" : [
            { "_id" : "AM", "longname" : "Angewandte Mathematik" }, 
            { "_id" : "POS", "longname" : "Programmieren und Software Engineering" }]})
```

Dies sieht auf den ersten Blick einmal mühsam aus, da das ganze Dokument zum Server gesendet
werden muss.

#### Vorteil

In Applikationen ist es bei der Bearbeitung von Datensätzen üblich, dem Client den
Datensatz zu senden. Danach sendet der Client die aktualisierten Felder über HTTP PUT an den
Server zurück. Nun kann das Dokument mit einer Zeile aktualisiert werden:

```c#
// Serialize newTeacher from HTTP Request (PUT formdata, ...)
db.getCollection("teachers").ReplaceOne(Builders<Teacher>.Filter.Eq(t => t.Id, newTeacher.Id), newTeacher);
```

Diese Anweisung ist zudem so allgemein, dass sie in einem generischen Repository implementiert
werden kann (suche nach dem Id Wert und ersetze das Dokument durch das neue Objekt).

#### Nachteil

Sehen wir uns das folgende Sequenzdiagramm an. 2 Clients wollen den Datensatz des Lehrers *SZ*
bearbeiten. Client 1 ändert die Mailadresse auf *new_mail@spengergasse.at*. Client 2 ändert
das Gehalt auf 2000.

![](replace_one_1700.png)

Bei der *updateOne()* Variante wird das Feld gesetzt, welches vom User geändert wurde. Die erste
Update Anweisung von Client 2 (er drückt zuerst auf speichern) ändert nur die Mailadresse. Client 1
übermittelt nun das neue Gehalt. Am Ende hat das Dokument den Wert
*{"_id": "SZ", "salary": 2000, "mail": "new_mail@spengergasse.at"}*.

Bei der *replaceOne()* Variante schreibt Schritt (6) zwar auch das neue Gehalt von 2000, nur
übermittelt der Client auch die alten Daten. Am Ende hat das Dokument den Wert
*{"_id": "SZ", "salary": 2000, "mail": "sz@spengergasse.at"}*. Dies bezeichnet man auch als
"last one wins".

Dieses Szenario hat nichts mit Transaktionen zu tun. Es entsteht einfach dadurch, dass der User
bei der Bearbeitung des Datensatzes die angezeigten Daten zum Zeitpunkt t in den Textfeldern
hat und diese Inhalte später wieder an den Server schickt.

Eine Lösung bieten auch OR Mapper in relationalen Datenbanken: Es wird ein Timestamp *lastUpdate*
gespeichert. Dieser wird mit an den Client gesendet. Schickt nun der Client die Daten an den
Server zurück, wird vor dem Update geprüft: Ist der übermittelte *lastUpdate* Wert gleich dem
aktuell gespeicherten? Wenn nein kann dieser Fall im Programm mit eigener Logik behandelt werden.

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
        // Im Semester 2023W ist das Anfangsdatum (start) auf den 4.09.2023 und das Endedatum (end)
        // auf den 5.02.2024 zu setzen. Setze beide Felder mit einer Anweisung.
        {
            PrintHeader("Start und Endedatum des Semesters 2023W wird auf den 4.9.2023 bzw. auf den 5.2.2024 gesetzt.");
            var result = db.GetCollection<Term>("terms")
                .UpdateOne(
                    Builders<Term>.Filter.Eq(t => t.Id, "2023W"),
                    Builders<Term>.Update.Combine(
                        Builders<Term>.Update.Set(t => t.Start, new DateOnly(2023, 9, 4)),
                        Builders<Term>.Update.Set(t => t.End, new DateOnly(2024, 2, 5))));
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
            Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
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

## Übung

Du kannst die folgende Aufgabe auf 2 Arten lösen:

1. Schreiben der Update Anweisungen in der Shell von Studio 3T.
2. Verwenden der Update Methode in .NET oder in Java

Die Aufgaben sind im untenstehenden Programmcode als Kommentar. Falls du die Aufgabe in
**.NET** lösen möchtest, gehe so vor: Kopiere das Generatorprogramm im Ordner
*\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner (z. B. *UpdateExcercise*).
Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt und schreibe deine Update
Anweisung an die Stelle von *TODO: Schreibe hier deine update Anweisung*.

Die Codeanweisungen danach (und manchmal auch davor) prüfen dein Update. Stimmen die Daten
nicht, erfolgt eine entsprechende rote Ausgabe in der Konsole.

Wenn du die Update Anweisungen in **Java** generieren möchtest, gibt es im
Ordner *13_NoSQL/examsDbClient_Java* ein Demoprogramm mit allen Modelklassen für den Zugriff
auf die Datenbank.

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


        // *****************************************************************************************
        // Muster: Der Raum AH.06 hat nach einer Zählung der Sessel eine Kapazität von 24 Plätzen. Setze
        // das Feld capacity für diesen Raum.
        {
            examsDb.Rooms.UpdateOne(
                Builders<Room>.Filter.Eq(t => t.Shortname, "AH.06"),
                Builders<Room>.Update.Set(t => t.Capacity, 24));
            CheckAndWrite(
                () => examsDb.Rooms.AsQueryable().First(r => r.Shortname == "AH.06").Capacity == 24,
                "Raum AH.06 hat 24 Plätze.");
        }

        // *****************************************************************************************
        // (1) Die Studentin mit der ID 100003 hat geheiratet und heißt nun Bayer statt Neubert.
        //     Aktualisiere das Feld name.lastname in der students collection. Die exams Collection
        //     ist NICHT zu aktualisieren, da sie bei der Prüfung ja noch Neubert geheißen hat.
        {
            // TODO: Schreibe hier deine update Anweisung.
            CheckAndWrite(
                () => examsDb.Students.AsQueryable().First(s => s.Id == 100003).Name.Lastname == "Bayer",
                "Die Studentin 100003 heißt Bayer.");
        }

        // *****************************************************************************************
        // (2) Das Semester mit der ID 2023S (Sommersemester 2023/24) endet nicht wie eingetragen am
        //     1.7.2024 sondern am 28.06.2024. Aktualisiere das Dokument in der terms Collection. Da
        //     das Document noch nirgends eingebettet wurde, muss es nur in der terms Collection
        //     geändert werden.
        {
            // TODO: Schreibe hier deine update Anweisung.
            CheckAndWrite(
                () => examsDb.Terms.AsQueryable().First(t => t.Id == "2023S").End == new DateOnly(2024, 6, 28),
                "Das Semester 2023S endet am 28.6.2024.");
        }

        // *****************************************************************************************
        // (3) Die Räume werden mit Sesseln aufgestockt. Jeder Raum, dessen Kapazität != null ist,
        //     soll 2 Plätze mehr im Feld capacity erhalten.
        {
            var oldCapacities = examsDb.Rooms.AsQueryable().Where(r => r.Capacity != null).ToDictionary(r => r.Shortname, r => r.Capacity);

            // TODO: Schreibe hier deine update Anweisung.

            var newCapacities = examsDb.Rooms.AsQueryable().Where(r => r.Capacity != null).ToDictionary(r => r.Shortname, r => r.Capacity);
            CheckAndWrite(
                () => oldCapacities.Count == newCapacities.Count && oldCapacities.All(r => newCapacities[r.Key] == r.Value + 2),
                "Die Räume haben nun 2 Plätze mehr.");
        }

        // *****************************************************************************************
        // (4) Die Studentin mit der ID 100019 ist ins Studentenheim gegenüber der Schule gezogen.
        //     Die neue Adresse ist
        //     street: Spengergasse, streetNr: 18, city: Wien, zip: 1050
        //     Hinweis: Erstelle ein neues Address Objekt und setze es als neuen Wert fest.

        {
            // TODO: Schreibe hier deine update Anweisung.
            CheckAndWrite(
                () =>
                {
                    var address = examsDb.Students.AsQueryable().First(s => s.Id == 100019).Address;
                    return address.City == "Wien" && address.Zip == 1050 && address.Street == "Spengergasse" && address.StreetNr == "18";
                },
                "Die Studentin 100019 hat die Adresse Spengergasse 18, 1050 Wien.");
        }

        // *****************************************************************************************
        // (5) Der Lehrer TAN hat nun eine Lehrbefähigung in DBI. Trage das neue Objekt vom
        //     Typ Subject ein. Lese vorher aus der Collection subjects das Document, bevor zu
        //     es zuweist (also keinen "hardcoded" Wert für longname verwenden!). Nur wenn du die
        //     Shell direkt verwendest, kann der Langname hardcoded eingefügt werden.
        {
            // TODO: Schreibe hier deine update Anweisung.
            CheckAndWrite(
                () =>
                    examsDb.Teachers.AsQueryable().First(t => t.Id == "TAN")
                        .CanTeachSubjects
                        .Any(s => s.Shortname == "DBI" && s.Longname == "Datenbanken und Informationssysteme"),
                "Lehrer TAN hat nun DBI im Array canTeachSubjects.");
        }

        // *****************************************************************************************
        // (6) Lehrplanwechsel: POS heißt nun Programmieren und Software Development
        //     (statt Engineering). Aktualisiere das Document in subjects und alle Einträge in
        //     canTeachSubject der Lehrenden.
        {
            var oldTeacherSubjects = examsDb.Teachers.AsQueryable().ToList()
                .SelectMany(t => t.CanTeachSubjects.Select(c => new { t.Id, c.Shortname, c.Longname }))
                .ToList();

            // TODO: Schreibe hier deine update Anweisung.

            var newTeacherSubjects = examsDb.Teachers.AsQueryable().ToList()
                .SelectMany(t => t.CanTeachSubjects.Select(c => new { t.Id, c.Shortname, c.Longname }))
                .ToList();

            CheckAndWrite(
                () => examsDb.Subjects.AsQueryable().First(s => s.Shortname == "POS").Longname == "Programmieren und Software Development",
                "POS wurde in der Collection subjects umbenannt.");
            CheckAndWrite(
                () => oldTeacherSubjects.Count == newTeacherSubjects.Count &&
                    newTeacherSubjects.Where(t => t.Shortname == "POS").All(t => t.Longname == "Programmieren und Software Development"),
                "POS wurde in allen Dokumenten der Collection teachers umbenannt.");
        }

        // *****************************************************************************************
        // (7) Die 4AAIF des heurigen Schuljahres (2022S_4AAIF) nun den Stammraum AH.06. Aktualisiere
        //     das Document in der classes Collection. Vergiss nicht, dass die Klasse auch als
        //     currentClass in der students und exams Collection vorkommt. Diese Klassen sollen ebenfalls
        //     aktualisiert werden.
        {
            // TODO: Schreibe hier deine update Anweisung.

            CheckAndWrite(
                () => examsDb.Classes.AsQueryable().First(r => r.Id == "2022S_4AAIF").RoomShortname == "AH.06",
                "Die 2022S_4AAIF hat den Stammraum AH.06.");
            CheckAndWrite(
                () => !examsDb.Students.AsQueryable().Where(s => s.CurrentClass!.Id == "2022S_4AAIF").Any(s => s.CurrentClass!.RoomShortname != "AH.06"),
                "Die 2022S_4AAIF hat den Stammraum AH.06. Das Feld currentClass in students wurde aktualisiert.");
            CheckAndWrite(
                () => !examsDb.Exams.AsQueryable().Where(e => e.CurrentClass.Id == "2022S_4AAIF").Any(e => e.CurrentClass!.RoomShortname != "AH.06"),
                "Die 2022S_4AAIF hat den Stammraum AH.06. Das Feld currentClass in exams wurde aktualisiert.");
        }

        // *****************************************************************************************
        // (8) Die Studentin mit der ID 100162 wechselt von der 8BBIF (2022S_8BBIF) in die Klasse
        //     2022S_8ABIF. Aktualisiere das Feld currentClass, indem du die Infos der Klasse
        //     2022S_8ABIF aus der classes Collection liest und zuweist. Füge dann einen neuen
        //     Eintrag im Array classHistory mit dieser Klasse hinzu.
        {
            var oldStudent = examsDb.Students.AsQueryable().First(s => s.Id == 100162);

            // TODO: Schreibe hier deine update Anweisung.

            var newStudent = examsDb.Students.AsQueryable().First(s => s.Id == 100162);
            CheckAndWrite(
                () => newStudent.CurrentClass!.Id == "2022S_8ABIF",
                "Die Studentin 100162 hat nun als currentClass die Klasse 2022S_8ABIF.");
            CheckAndWrite(
                () => newStudent.ClassHistory.Count == oldStudent.ClassHistory.Count + 1 &&
                    newStudent.ClassHistory.Any(c => c.Id == "2022S_8ABIF"),
                "Die Studentin 100162 hat die Klasse 2022S_8ABIF in der classHistory.");
        }

        return 0;
    }

    static void CheckAndWrite(Func<bool> predicate, string message, int weight = 1)
    {
        var color = Console.ForegroundColor;
        try
        {
            if (predicate())
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"OK: {message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Fehler: {message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception {e.Message}: {message}");
        }
        Console.ForegroundColor = color;
    }
}

```
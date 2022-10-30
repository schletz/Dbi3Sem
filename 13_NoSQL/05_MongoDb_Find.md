# Filtern von Collections

So wie in jeder anderen Datenbank ist in MongoDB das Filtern von Daten ein zentraler Punkt. Wir
haben mehrere Möglichkeiten, Daten zu filtern:

- Mit der Shell in Studio 3T (oder der Mongo Shell).
- In einer Applikation mittels dem MongoDB Treiber.

## Filtern mit der Shell in Studio 3T

Nachdem du dich mit der Datenbank *examsDb* in Studio 3T verbunden hast, kannst du die ersten
Filterbefehle eingeben.

![](studio3t_shell_2122.png)

## Die verschiedenen Filter

Die nachfolgenden Filter werden der Funktion *db.getCollection(collectionname).find()* als
Parameter übergeben. Möchtest du z. B. einen leeren Filter in der Shell von Studio 3T absetzen,
dann schreibe folgenden Ausdruck:

```javascript
db.getCollection("teachers").find({})
```

Die nachfolgenden Beispiele verwenden - wenn nicht anders angegeben - die Collection *teachers*.

### Der leere Filter: {}

Möchten wir alle Dokumente einer Collection bekommen, verwenden wir den leeren Filter `{}`.
So liefern die folgenden Anweisungen in der Mongo Shell alle Documents der Collection *teachers*:

### Equals Filter { "field" : "value" }

Entspricht in SQL der Klausel *WHERE Col = Value*. Wenn wir ein Feld mit einem literal (fixen Wert)
vergleichen möchten, geben wir z. B. folgende Filter an:

- **{ "_id" : "RAU" }**   
  Sucht nach Dokumenten, wo das Feld *_id* den Wert *RAU* hat.
  Entspricht *_id = "RAU"*.
- **{ "name.email": "rau@spengergasse.at" }**   
  Wir können auch in Objekten innerhalb des Dokumentes 
  mit dem Punkt suchen. Dieser Filter sucht nach Dokumenten, wo das Feld *name.email* den Wert
  *rau@spengergasse.at* hat.
- **{ "hoursPerWeek" : {"$exists" : true } }**   
  Findet alle Dokumente, die das Feld *hoursPerWeek* besitzen. In unserer Datenbank werden *null*
  Werte nicht geschrieben, deswegen sind alle Werte, die wir bekommen, auch ungleich *null*. Das
  muss aber nicht so sein, *$exists* liefert grundsätzlich auch Felder, die den Wert *null* haben.
- **{ "homeOfficeDays": "MO" }**   
  *homeOfficeDays* ist ein Stringarray. Geben wir es als Feld an,
  werden alle Dokumente geliefert, die den angegebenen Wert (unter anderem) im Array haben.
- **{ "canTeachSubjects._id": "POS" }**  
  *canTeachSubjects* ist ein Array von Objekten. Wir können auf ein Feld im Objekt mit dem Punkt
  einfach zugreifen.

In der Shell werden diese Filter der *find* Funktion übergeben. Beispiel:

```javascript
db.getCollection("teachers").find({ "_id" : "RAU" })
```

### Not Equals Filter { "field" : { "$ne" : "value" } }

Entspricht in SQL der Klausel *WHERE Col <> Value*. Wir können also folgendes herausfiltern:

- **{ "hoursPerWeek" : {"$ne" : null } }**   
  Findet alle Dokumente, die einen Wert im Feld *hoursPerWeek* haben. Wenn das Feld *hoursPerWeek*
  im Dokument nicht vorkommt, wird der Datensatz auch nicht geliefert.

- **{ "homeOfficeDays" : { "$ne" : "MO" } }**   
  Findet alle Dokumente, die den Wert *MO* nicht im Array *homeOfficeDays* haben. Dokumente mit
  leererm Array werden zurückgegeben (sie haben ja nicht *MO* in der Liste).

### Greater/Lower Filter { "field" : { "$gt" : "value" } } oder { "field" : { "$lt" : "value" } }

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
- **{"dateTime":{"$lt": Date("2020-02-24T09:00:00Z")}}** (exams Collection)   
  Findet alle Dokumente, wo der Wert von *dateTime* kleiner als der 24.2.2020
  um 9 Uhr UTC ist. Es muss mit der *Date* Funktion der String in einen Zeitstempel umgewandelt werden.
  Versuche auch den Suchfilter *{"dateTime":{"$lt": Date("2020-02-24T08:30:00+01:00")}}*
  Er gibt an, welche Prüfungen vor dem 24.2.2020 um 8:30 MEZ (UTC+1h) statt gefunden haben.

### Mehrere Filter mit AND verknüpfen: { "field1": Filter1, "field2": Filter2 }

Entspricht in SQL der Klausel *WHERE Col1 = Value1 AND Col2 = Value2*.

- **{ "_id" : "RAU", "name.email" : "rau@spengergasse.at" }**   
  Findet alle Dokumente, wo das Feld *_id* den Wert RAU und das Feld *name.email* den Wert rau@spengergasse.at hat.
- **{ "hoursPerWeek": { "&dollar;gt" : 16 }, "salary": { "&dollar;gt" : 3000 } }**   
  Entspricht *hoursPerWeek > 16 AND salary > 3000*.

### Mehrere Filter mit OR verknüpfen: { "$or" : [{  "field1": Filter1 }, {  "field2": Filter2 }] }

Entspricht in SQL der Klausel *WHERE Col1 = Value1 OR Col2 = Value2*.

- **{ "$or" : [{ "_id" : "RAU" }, { "name.email" : "rau@spengergasse.at" }] }**   
  Entspricht der Abfrage *_id = "RAU" OR name.email = "rau@spengergasse.at"*.
- **{ "&dollar;or" : [{ "salary" : { "&dollar;gt" : 4000 } }, { "hoursPerWeek" : { "$lt" : 10 } }] }**   
  Entspricht der Abfrage *salary > 4000 OR hoursPerWeek < 10*.

### Der IN Filter: { "$in": [value1, value2, ...] }

Entspricht in SQL der Klausel *WHERE Col1 IN (Value1, Value2, ...)* und prüft, ob der Wert des
Feldes in der übergebenen Werteliste vorhanden ist.

- **{ "_id": { "$in": ["RAU", "SAC"] } }**   
  Findet alle Dokumente, dessen Feld *_id* RAU oder SAC ist. Der $in Operator verknüpft also
  alle Elemente mit OR.
- **{ "homeOfficeDays" : {"$in": ["MO", "MI"] } }**   
  Ermittelt alle Dokumente, die den Wert MO der den Wert MI im Array *homeOfficeDays* haben.

### Spezielle Filter für Arrays

#### Der elemMatch Filter { "array" : { "$elemMatch" : { query1, query2, ... } } }

- **{ "homeOfficeDays": { "&dollar;elemMatch": { "&dollar;ne": "MO" } } }**   
  Der Filter *{ "homeOfficeDays" : { "$ne" : "MO" } }* wurde schon diskutiert: Er liefert alle
  Dokumente, die den MO nicht als Wert im Array *homeOfficeDays* haben. Somit werden Dokumente mit
  {DI, MI}, {}, ... geliefert, jedoch kein Dokument mit {MO}, {MO, MI}, ...

  Nun wollen wir ermitteln, welche Lehrenden einen Tag im Array haben, der nicht der Montag (MO)
  ist. Das ist ein anderer Sachverhalt. Die Arrays {DI, MI}, {}, {MO, MI} sollen geliefert werden,
  während {MO} ausgeschlossen werden soll (das Array hat keinen Tag ungleich Montag). Der
  angezeigte Filter liefert dieses Ergebnis.

  Der *$elemMatch* Operator wird also wie der Name schon sagt *pro Element* und liefert true, wenn
  irgendein Element dem Kriterium entspricht.

  Beachte: Dokumente mit leerem *homeOfficeDays* Array werden nicht geliefert.
  > The $elemMatch operator matches documents that contain an array field with at least one
  > element that matches all the specified query criteria.
  > <sup>https://www.mongodb.com/docs/manual/reference/operator/query/elemMatch/</sup>


#### Der all Filter { "array" : { "$all" : ["value1", "value2"] } }

Prüft, ob alle *übergebenen Werte* im gespeicherten Array vorkommen.

- **{ "homeOfficeDays" : { "$all" : ["MO", "FR"] } }**   
  Liefert alle Documents, wo das Array *homeOfficeDays* die Werte *MO* und *FR* besitzt.
  *{"MO", "DI"}* wird also *nicht* geliefert. *{"MO", "DI", "FR"}* wird zurückgeliefert.
- **{ "canTeachSubjects._id" : { "$all" : ["POS", "DBI"] } }**   
  Betrachtet für den Vergleich das Feld *_id* der Objekte im Array *canTeachSubects*. Es
  werden alle Documents geliefert, die sowohl POS als auch DBI als Wert *_id* im Array
  *canTeachSubjects* haben.

#### Filtern nach der Größe des Arrays

Oft wollen wir wissen, ob ein Array im Dokument eine gewisse Größe hat oder überhaupt Werte
besitzt.

- **{ "homeOfficeDays.1": { "$exists": true } }**  
  Liefert alle Dokumente, wo das Array *homeOfficeDays* 2 oder mehr Werte hat.
  Dieser Filter sieht sehr seltsam aus, denn er funktioniert mit einem Trick. *homeOfficeDays.1*
  gibt den Index 1 des Arrays *homeOfficeDays* zurück, also *homeOfficeDays[1]*. Wie in Java oder C#
  startet der Index bei 0. Somit sagt der Filter aus: Wo existiert ein zweiter Eintrag im Array?
  Das können nur Arrays sein, die 2 oder mehr Werte haben.
- **{ "homeOfficeDays.0": { "$exists": true } }**  
  Analog zum vorigen Beispiel liefert dieser Filter alle Dokumente, wo das Array *homeOfficeDays*
  mindestens einen Wert besitzt. Wird der Wert von *$exists* auf *false* gesetzt, werden alle
  Dokumente geliefert, die keinen Eintrag im Array *homeOfficeDays* haben.

### Filter mit regulären Ausdrücken { "field" : /RegExp/ }

Als Beispiel verwenden wir die Collection *classes*, um nach Klassen zu filtern:

- **{ "_id" : /3.AIF/ }**   
  Findet die IDs 2019W_3AAIF, 2019W_3BAIF, 2020W_3AAIF, 2020W_3BAIF, 2021W_3AAIF, 2021W_3BAIF, 2022W_3AAIF, 2022W_3BAIF
- **{ "_id" : /^2022[WS]_3.AIF$/ }**   
  Findet die IDs 2022W_3AAIF, 2022W_3BAIF

### Der flexible where Filter: {"$where": "Expression"}

Der *where* Filter kann Javascript Ausdrücke verarbeiten. Dadurch können wir auch mehrere
Felder eines Documents miteinander vergleichen.

- **{"$where": "this._id == 'ZIP'"}**   
  Da wir das doppelte Anführungszeichen außen verwenden, verwenden wir im Ausdruck das einfache
  Anführungszeichen. Mit *this* werden die Felder des Documents angesprochen.
- **{"$where": "this.hoursPerWeek > 16"}**     
- **{"$where": "this.hoursPerWeek != null && this.salary > 500 * this.hoursPerWeek"}**   
  Wer verdient über 500 Euro pro geleisteter Wochenstunde? Wir können die aus JavaScript bekannten
  und bzw. oder Operatoren verwenden.

Aus Performancegründen sollten jedoch die oben beschriebenen Operatoren verwendet werden
<sup>https://www.mongodb.com/docs/manual/reference/operator/aggregation/function/#example-2--alternative-to--where</sup>.
Falls eine JavaScript Funktion verwendet werden muss, ist der Operator *$function* zu bevorzugen. Er
ist im Kapitel [Aggregations](06_MongoDb_Aggregate.md) beschrieben.

## Filtern mit dem .NET MongoDB Treiber

Das nachfolgende Musterprogramm zeigt, wie diese Filterausdrücke in .NET erzeugt werden können. Es
stehen 2 Varianten zur Verfügung:

- Der Filter Builder. Hier können die Filterausdrücke aufgebaut werden. Der Builder orientiert sich
  sehr nahe an der Syntax der Filter.
- Mit *AsQueryable()*. Hier kann LINQ zur Filterung verwendet werden. Es kann allerdings nicht
  jeder Filterausdruck so generiert werden. Manchmal muss der Builder verwendet werden.

### Mit dem Builder

Der Builder generiert einen Suchfilter mit dem entsprechenden Operator. Im folgenden Beispiel
wird nach Räumen mit der Kapazität von 29 Plätzen gesucht:

```c#
var results = db.GetCollection<Room>("rooms")
    .Find(Builders<Room>.Filter.Eq(r => r.Capacity, 29))
    .ToList();

foreach (var result in results.ToList())
{
    Console.WriteLine($"{result.Shortname} hat eine Kapazität von {result.Capacity} Plätzen.");
}

```

*Eq* ist der Operator (Equals). Er bekommt 2 Parameter: Das Feld, nach dem wir filtern möchten als
Lambda Expression. Der 2. Parameter ist der Wert, der gesucht werden soll. Die nachfolgende *Find()*
Methode bekommt diesen Suchfilter. Es wird allerdings noch nichts abgefragt. Erst mit *ToList()*
bzw. *FirstOrDefault()* werden die Daten in den Speicher geladen und können verwendet werden.

### Mit AsQueryable()

Wer schon mit LINQ gearbeitet hat, findet folgenden Zugang vertrauter:

```c#
var results = db.GetCollection<Room>("rooms")
    .AsQueryable()
    .Where(r => r.Capacity == 29)
    .ToList();
foreach (var result in results)
{
    Console.WriteLine($"{result.Shortname} hat eine Kapazität von {result.Capacity} Plätzen.");
}
```

Die Methode *AsQueryable()* liefert den Typ *IMongoQueryable* zurück, welcher die LINQ Methoden
in MongoDB Ausdrücke umwandelt. Dadurch können die gewohnten Funktionen wie *Where()*, *Select()*,
*GroupBy()*, ... verwendet werden.

### Demoprogramm

Kopiere das Generatorprogramm im Ordner *\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner
(z. B. *FilterDemos*). Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt.
Das Programm gibt die Filter, die es generiert, aus. *AsQueryable()* erzeugt eine *aggregate()*
Anweisung. Diese kann einmal ignoriert werden, das wird im Kapitel *Aggregate* genauer diskutiert.

**Program.cs**
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
            // find({ "hoursPerWeek" : { "$exists" : true } })
            PrintHeader("Lehrer, die das Feld hoursPerWeek besitzen.");
            var filter = Builders<Teacher>.Filter.Exists(t => t.HoursPerWeek);
            var results = db.GetCollection<Teacher>("teachers").Find(filter).ToList();
            Console.WriteLine(string.Join(", ", results.Select(r => r.Id)));
            // Alternativ: Variante mit AsQueryable(). Wir vergleichen auf != null, da bei nicht
            // existierenden Feldern der default Wert gesetzt wird.
            var results2 = db.GetCollection<Teacher>("teachers").AsQueryable()
                .Where(t => t.HoursPerWeek != null)
                .ToList();
            Console.WriteLine(string.Join(", ", results2.Select(r => r.Id)));
        }

        {
            // *************************************************************************************
            // find({ "hoursPerWeek": { "$gt" : 16 }, "salary": { "$gt" : 3000 } })
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
            var filter = Builders<Class>.Filter.Regex(c => c.Id, @"3.AIF");
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
            // find({ "$where" : "this.salary > NumberDecimal(500 * this.hoursPerWeek)" })
            PrintHeader("Lehrer, die mehr als 500 Euro pro Wochenstunde bekommen (Gehalt > 500 * hoursPerWeek).");
            // Achte auf die Anführungszeichen! @ bedeutet verbatim string (kein Escapen von Sonderzeichen).
            // Im Inneren müssen die Anführungszeichen doppelt geschrieben werden (" --> "")
            var results = db.GetCollection<Teacher>("teachers").Find(@"{$where: ""this.salary > NumberDecimal(500 * this.hoursPerWeek)""}").ToList();
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

## Filtern mit dem Java MongoDB Treiber

Kopiere das Programm im Ordner *13_NoSQL/examsdb-java* in ein neues Verzeichnis. Ersetze danach
die Datei *Main.java* durch die untenstehende Version. Sie zeigt, wie die oben verwendeten Filter
in Java geschrieben werden können.

**Main.java**
```java
package at.spengergasse.examsdb;

import java.util.ArrayList;
import java.util.stream.Collectors;

import com.mongodb.MongoSecurityException;
import com.mongodb.MongoTimeoutException;
import com.mongodb.client.model.Filters;

import at.spengergasse.examsdb.infrastructure.ExamDatabase;

public class Main {
    public static void main(String[] args) {
        var examDatabase = ExamDatabase.fromConnectionString("mongodb://root:1234@localhost:27017", false);
        try {
            examDatabase.Seed();
        } catch (MongoTimeoutException e) {
            System.err.println("Die Datenbank ist nicht erreichbar. Läuft der Container?");
            System.exit(1);
            return;
        } catch (MongoSecurityException e) {
            System.err.println("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
            System.exit(2);
            return;
        }

        catch (Exception e) {
            System.err.println(e.getMessage());
            System.exit(3);
            return;
        }

        // Für den leichteren Zugriff auf die Collections stellt die Klasse ExamDatabase
        // folgende Methoden bereit:
        // MongoCollection<SchoolClass> examDatabase.getClasses()
        // MongoCollection<Exam> examDatabase.getExams()
        // MongoCollection<Room> examDatabase.getRooms()
        // MongoCollection<Student> examDatabase.getStudents()
        // MongoCollection<Subject> examDatabase.getSubjects()
        // MongoCollection<Teacher> examDatabase.getTeachers()
        // MongoCollection<Term> examDatabase.getTerms()

        // Hinweis: Setze den Parameter enableLogging in Zeile 22 auf true, um die gesendeten
        // Befehle anzuzeigen.
        {
            System.out.println("Lehrer mit der ID RAU");
            var result = examDatabase.getTeachers()
                    .find(Filters.eq("_id", "RAU"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Lehrer, die das Feld hoursPerWeek besitzen.");
            var result = examDatabase.getTeachers()
                    .find(Filters.exists("hoursPerWeek", true))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Lehrer mit über 16 Stunden im Feld hoursPerWeek und einem Gehalt von über 3000 EUR.");
            var result = examDatabase.getTeachers()
                    .find(Filters.and(
                        Filters.gt("hoursPerWeek", 16),
                        Filters.gt("salary", 3000)))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }       
        {
            System.out.println("Welche Lehrer haben am MO einen Home Office Tag (MO ist im Array homeOfficeDays enthalten)?");
            var result = examDatabase.getTeachers()
                    .find(Filters.eq("homeOfficeDays", "MO"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Welche Lehrer haben am MO und am FR einen Home Office Tag (MO UND FR ist im Array homeOfficeDays enthalten)?");
            var result = examDatabase.getTeachers()
                    .find(Filters.all("homeOfficeDays", new String[]{"MO", "FR"}))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }         
        {
            System.out.println("Welche Lehrer können POS und DBI unterrichten (POS UND DBI ist im Array canTeachSubjects als Id enthalten)?");
            var result = examDatabase.getTeachers()
                    .find(Filters.all("canTeachSubjects._id", new String[] { "POS", "DBI" }))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Alle 3. Semester AIF Klassen in der Collection classes.");
            var result = examDatabase.getClasses()
                    .find(Filters.regex("_id", "3.AIF"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Alle 3. Semester AIF Klassen des Schuljahres 2022 in der Collection classes.");
            var result = examDatabase.getClasses()
                    .find(Filters.regex("_id", "^2022[WS]_3.AIF$"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
        {
            System.out.println("Lehrer, die mehr als 500 Euro pro Wochenstunde bekommen (Gehalt > 500 * hoursPerWeek).");
            var result = examDatabase.getTeachers()
                    .find(Filters.where("this.salary > NumberDecimal(500 * this.hoursPerWeek)"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                    .map(r -> r.id().toString())
                    .collect(Collectors.joining(", ")));
        }
    }
}
```

## Übung

Du kannst die folgende Aufgabe auf 3 Arten lösen:

1. Schreiben der Filter in der Shell von Studio 3T.
2. Verwenden des .NET Treibers mit *AsQueryable()* (wenn möglich) oder dem Filter Builder.
3. Verwenden des Java Treibers.

Die korrekten Ergebnisse sind unter den Beispielen. Aus Platzgründen sind nur die IDs abgebildet,
bei den Abfragen wird natürlich das ganze Dokument zurückgegeben. Ist der Key eine ObjectId, werden
nur die letzten 4 Bytes abgebildet.

Falls du die Aufgabe in **.NET** lösen möchtest, gehe so vor: Kopiere das Generatorprogramm im Ordner
*\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner (z. B. *FilterExcercise*).
Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt und schreibe das Ergebnis
deiner Abfrage in die Variable *result*.

Falls du die Aufgabe in **Java** lösen möchtest, gehe so vor: Kopiere das Generatorprogramm im Ordner
*13_NoSQL/examsdb-java* zuerst in einen eigenen Ordner (z. B. *FilterExcercise*).
Ersetze danach die Datei *Main.java* durch den folgenden Inhalt und schreibe das Ergebnis
deiner Abfrage in die Variable *result*. 

**(1)** Welche Klassen gibt es in der Classes Collection im Schuljahr 2021 (Year ist 2021)
der Abteilung CIF?

```
2021S_4ACIF, 2021S_4BCIF, 2021S_6ACIF, 2021S_6BCIF, 2021S_8ACIF, 2021S_8BCIF, 2021W_3ACIF, 
2021W_3BCIF, 2021W_5ACIF, 2021W_5BCIF, 2021W_7ACIF, 2021W_7BCIF
```

**(2)** Welche Räume haben eine Kapazität von 30 oder mehr Plätzen (Capacity)?

```
AH.14, B2.10
```

**(3)** Gib alle Klassen ab dem 5. Semester der KIF Abteilung im Jahr 2022 aus (EducationLevel >= 5).

```
2022S_6AKIF, 2022S_6BKIF, 2022W_5AKIF, 2022W_5BKIF
```

**(4)** Welche Studierenden sind vor dem 1.1.1991 geboren?

```
100072, 100075, 100266, 100271, 100274, 100281, 100459, 100470, 100476
```

**(5)** Welche Klassen des Wintersemesters 2022 haben keinen Stammraum (RoomShortname ist null)?

```
2022W_3AAIF, 2022W_3ACIF, 2022W_3BKIF, 2022W_5ABIF, 2022W_7ABIF
```

**(6)** Welche negativen Prüfungen gab es zwischen 1.1.2022 und 27.1.2022?
Erstelle mit new DateTime(Year, Month, Day) einen Datums/Zeitwert. Beachte, dass das Endedatum als "kleiner" Filter und mit 28.1.2022
gesetzt werden muss. Die erstellten DateTime Werte haben nämlich als Zeitwert 0:00, sonst werden die Prüfungen am letzten
Tag nicht gelistet!

```
00000024, 0000007f, 000000c3, 000000e9, 00000136, 00000175
```

**(7)** Welche Lehrer dürfen das Fach POS unterrichten (haben also POS in der Liste CanTeachSubjects)?

```
CAM, DAU, HAR, KNE, KUR, NOR, SCH, THR
```

**(8)** Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF besucht?

```
100421, 100422, 100423, 100424, 100425, 100426, 100427, 100428, 100429, 100430, 100431, 100432, 
100433, 100434, 100435, 100436, 100437, 100438, 100439, 100440, 100441, 100442, 100443, 100444, 
100445, 100446, 100447
```

**(9)** Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF,
aber 2022/23 nicht die 5BKIF besucht?

```
100427, 100429, 100430, 100431, 100438, 100443
```

**(10)** Welche Prüfungen erreichten maximal 25% der Punkte (Points ist also <= PointsMax * 0.25)?
Hinweis: Versuche, ob deine Lösung durch den Builder erzeugt werden kann. Wenn nicht,
kannst du einen where Filter wie im Beispiel als String verwenden.  

```
00000007, 0000001a, 0000001f, 0000002b, 0000002e, 0000005a, 0000007a, 0000007e, 00000083, 00000092,
0000009c, 0000009e, 000000a3, 000000c0, 000000c8, 000000d6, 000000ff, 00000100, 00000111, 00000156
```

**Program.cs (.NET)**
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
        //      Hinweis: Versuche, ob deine Lösung durch den Builder erzeugt werden kann. Wenn nicht,
        //      kannst du einen where Filter wie im Beispiel als String verwenden.   
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

**Main.java (Java)**
```java
package at.spengergasse.examsdb;

import java.util.ArrayList;
import java.util.stream.Collectors;

import com.mongodb.MongoSecurityException;
import com.mongodb.MongoTimeoutException;
import com.mongodb.client.model.Filters;

import at.spengergasse.examsdb.infrastructure.ExamDatabase;
import at.spengergasse.examsdb.model.Exam;
import at.spengergasse.examsdb.model.Room;
import at.spengergasse.examsdb.model.SchoolClass;
import at.spengergasse.examsdb.model.Student;
import at.spengergasse.examsdb.model.Teacher;

public class Main {
    public static void main(String[] args) {
        var examDatabase = ExamDatabase.fromConnectionString("mongodb://root:1234@localhost:27017", false);
        try {
            examDatabase.Seed();
        } catch (MongoTimeoutException e) {
            System.err.println("Die Datenbank ist nicht erreichbar. Läuft der Container?");
            System.exit(1);
            return;
        } catch (MongoSecurityException e) {
            System.err.println("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
            System.exit(2);
            return;
        }

        catch (Exception e) {
            System.err.println(e.getMessage());
            System.exit(3);
            return;
        }

        // Für den leichteren Zugriff auf die Collections stellt die Klasse ExamDatabase
        // folgende Methoden bereit:
        // MongoCollection<SchoolClass> examDatabase.getClasses()
        // MongoCollection<Exam> examDatabase.getExams()
        // MongoCollection<Room> examDatabase.getRooms()
        // MongoCollection<Student> examDatabase.getStudents()
        // MongoCollection<Subject> examDatabase.getSubjects()
        // MongoCollection<Teacher> examDatabase.getTeachers()
        // MongoCollection<Term> examDatabase.getTerms()

        // Hinweis: Setze den Parameter enableLogging in Zeile 22 auf true, um die
        // gesendeten
        // Befehle anzuzeigen.
        {
            System.out.println("MUSTER: Weibliche Studierende der 6AAIF");
            var result = examDatabase.getStudents()
                    .find(Filters.and(
                            Filters.eq("gender", "Female"),
                            Filters.eq("currentClass.shortname", "6AAIF")))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream()
                            .map(r -> Integer.toString(r.id()))
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(1) Welche Klassen gibt es in der Classes Collection im Schuljahr 2021 (Year ist 2021) der Abteilung CIF?");
            var result = new ArrayList<SchoolClass>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id())
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(2) Welche Räume haben eine Kapazität von 30 oder mehr Plätzen (Capacity)?");
            var result = new ArrayList<Room>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.shortname())
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(3) Gib alle Klassen ab dem 5. Semester der KIF Abteilung im Jahr 2022 aus (EducationLevel >= 5).");
            var result = new ArrayList<SchoolClass>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id())
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(4) Welche Studierenden sind vor dem 1.1.1991 geboren?");
            var result = new ArrayList<Student>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> Integer.toString(r.id()))
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(5) Welche Klassen des Wintersemesters 2022 haben keinen Stammraum (RoomShortname ist null)?");
            var result = new ArrayList<SchoolClass>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id())
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(6) Welche negativen Prüfungen gab es zwischen 1.1.2022 und 27.1.2022?");
            var result = new ArrayList<Exam>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id().toHexString().substring(16, 4))
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(7) Welche Lehrer dürfen das Fach POS unterrichten (haben also POS in der Liste CanTeachSubjects)?");
            var result = new ArrayList<Teacher>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id())
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(8) Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF besucht?");
            var result = new ArrayList<Student>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> Integer.toString(r.id()))
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(9) Welche Studierende haben im Schuljahr 2021/22 (Year ist 2021) die 3BKIF, aber 2022/23 nicht die 5BKIF besucht?");
            var result = new ArrayList<Student>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> Integer.toString(r.id()))
                            .collect(Collectors.joining(", ")));
        }
        {
            System.out.println(
                    "(10) Welche Prüfungen erreichten maximal 25% der Punkte (Points ist also <= PointsMax * 0.25)?");
            var result = new ArrayList<Exam>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream()
                            .map(r -> r.id().toHexString().substring(16, 8))
                            .collect(Collectors.joining(", ")));
        }

    }
}

```

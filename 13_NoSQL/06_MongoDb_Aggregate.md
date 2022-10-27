# Aggregation und Pipelines

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

## Pipelines und die Funktion *aggregate()*

In der SQL Ausbildung kommt man recht schnell zu dem Punkt, wo wir Daten *gruppieren* und
*projizieren* möchten. Das folgende Kommando würde die Anzahl der positiven Prüfungsnoten
(Note < 5) ermitteln:

![](aggregation_sql_statement1016.svg)

Wir können das *SELECT* Statement in folgende Teile aufteilen:

- **Filterung** durch die *WHERE* Klausel. Vor der Gruppierung werden nur die entsprechenden
  Daten berücksichtigt. In diesem Fall sind es alle positiven Noten (Grade < 5).
- **Gruppierung** durch die *GROUP BY* Klausel. Dieser Ausdruck gibt an, auf Basis welcher
  Spalten die Gruppen gebildet werden sollen. Hier sind es Gruppen mit den gleichen Werten von
  *Grade*.
- **Aggregatsfunktionen** wie *COUNT* können die Daten pro Gruppe auf einen Wert aggregieren (zusammenführen).
  Hier ist es die Anzahl.
- **Projektion** in *SELECT* gibt an, welche Spalten wir zurückbekommen wollen. Die Aggregatsfunktion
  wird als Spalte mit einem Alias geschrieben.

In MongoDB werden solche Funktionen auch bereitgestellt. Der Befehl ist zwar anders aufgebaut,
hat aber die selben Elemente. Sehen wir uns den entsprechenden Befehl für das obere Beispiel an:

```javascript
db.getCollection("exams").aggregate([
  { "$match" :   { "grade" : { "$lt" : 5 } } },
  { "$group" :   { "_id" : "$grade", "count" : { "$sum" : 1 } } },
  { "$project" : { "grade" : "$_id", "count" : "$count" } }
])
```

Die Datenbank antwortet mit der Anzahl der Werte pro *grade*:

```javascript
[
  { _id: 4, grade: 4, count: 66 },
  { _id: 2, grade: 2, count: 52 },
  { _id: 1, grade: 1, count: 72 },
  { _id: 3, grade: 3, count: 61 }
]
```

Auch wenn die *aggregate()* Funktion scheinbar ganz anders als das SQL Statement aufgebaut ist,
gibt es doch Gemeinsamkeiten:

![](aggregation_aggregate_function_1035.svg)

Wir erkennen folgende Dinge:

- Der Parameter der *aggregate()* Funktion ist ein **Array**. Dies bezeichnet man auch als
  **Pipeline**, und jedes Element dieses Arrays ist ein Schritt. In einer Pipeline können wir im
  nächsten Schritt auf die Felder, die der vorige Schritt berechnet, zugreifen.
  Wir bezeichnen die einzelnen Schritte der Pipeline auch als *Stages*.
- **(1) Filterung:** Sie wird mit dem *$match* Operator gekennzeichnet. Das kennen wir schon von
  *updateOne()* und *updateMany()*. Mit den bekannten Filteroperationen können die entsprechenden
  Daten für die nächsten Schritte ausgewählt werden.
- **(2) Gruppierung und Aggregierung:** Im Gegensatz zu SQL definieren wir hier 2 Schritte auf
  einmal. Mit dem *"$group"* Operator definieren wir diesen Schritt.
  1. Mit einem *_id* Feld definieren wir das Felder, nach denen gruppiert werden soll. Es
     ist mit einem Dollar ($) gekennzeichnet.
  2. Danach geben wir die Aggregierungen an. Der Aufbau ist *{"name": "operation"}*. Der hier
     definierte Name ist frei wählbar, wir können in den nachfolgenden Schritten darauf zugreifen.
     In unserem Beispiel nennen wir das erzeugte Feld *count*. Der Ausdruck *{ "$sum" : 1 }*
     sieht etwas komisch aus, hat aber Logik. Es wird für jedes Dokument der Wert 1 aufsummiert.
     Somit erhalten wir die Anzahl.
- **(3) Projektion:** Mit *"$project"* definieren wir, welche Werte wir zurückgeben wollen.
  Wir können auf die vorher ermittelten Werte mit dem Dollar ($) Zeichen zugreifen (*$_id*, *$count*).


Möchten wir die ID Spalte (also die Spalte, nach der wir gruppiert haben) nicht ausgeben,
können wir die Ausgabe bei *project* auf 0 setzen.

```javascript
db.getCollection("exams").aggregate([
  { "$match" :   { "grade" : { "$lt" : 5 } } },  
  { "$group" :   { "_id" : "$grade", "count" : { "$sum" : 1 } } },
  { "$project" : { "Grade" : "$_id", "Count" : "$count", "_id" : 0 } }
])

[
  { Grade: 4, Count: 66 },
  { Grade: 2, Count: 52 },
  { Grade: 1, Count: 72 },
  { Grade: 3, Count: 61 }
]
```

### Aufbau der Pipeline im MongoDB Compass

Im Programm MongoDB Compass gibt es die Möglichkeit, die Pipeline Schritt für Schritt aufzubauen.
Es werden dann die Zwischenergebnisse angezeigt:

![](aggregation_compass_1208.svg)

## Aggregieren nach mehreren Feldern

In SQL ist es möglich, auch mehrere Spalten für die Gruppierung zu verwenden. Eine Gruppe wird dann
gebildet, wenn die Werte aller Spalten ident sind. Dadurch können wir die Anzahl der Prüfungen
pro Student und Subject ausgeben:

```sql
SELECT e.StudentNr, e.SubjectId, COUNT(*) AS Count
FROM   Exams
GROUP BY e.StudentNr, e.SubjectId
```

Bei *aggregate()* können wir einfach als *_id* Wert bei der Stage *$group* ein Objekt angeben.
Wir geben für jede Spalte einen Namen (hier *subject* und *studentNr*). Als Wert verweisen wir mit
Dollar ($) auf das Feld im Dokument:   
*{ "subject" : "$subject._id", "studentNr" : "$student.nr" }*

Das ganze Kommando sieht dann so aus. Da die Felder der Gruppierung automatisch als *_id* ausgeben
werden, schreiben wir sie nicht in die *$projekt* Operation.

```javascript
db.getCollection("exams").aggregate([
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "count" : { "$sum" : 1 } } },
  { "$project" : { "count" : "$count" } }
])

[
  { _id: { subject: 'DBI', studentNr: 100004 }, count: 1 },
  { _id: { subject: 'AM', studentNr: 100289 }, count: 2 },
  { _id: { subject: 'D', studentNr: 100596 }, count: 1 },
  { _id: { subject: 'AM', studentNr: 100639 }, count: 2 },
  // ...
]
```

Wollen wir statt einem verschachteltem Objekt die Felder der Gruppierung (*studentNr* und *subject*)
direkt ausgeben, können wir bei der Stage *$project* mit *"$_id.studentNr"* darauf zugreifen.
Das Feld *_id* geben wir dann nicht mehr aus, d. h. wir setzen es in der Projektion auf 0.

```javascript
db.getCollection("exams").aggregate([
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "count" : { "$sum" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$count", "_id" : 0 } }
])

[
  { studentNr: 100004, subject: 'DBI', count: 1 },
  { studentNr: 100289, subject: 'AM', count: 2 },
  { studentNr: 100596, subject: 'D', count: 1 },
  { studentNr: 100639, subject: 'AM', count: 2 },
  // ...
]
```

## Filtern nach der Gruppierung (HAVING in SQL)

In SQL wird zwischen der Filterung *vor* der Gruppierung mit *WHERE* und der Filterung *nach*
der Gruppierung mit *HAVING* unterschieden. Wollen wir alle Studierenden, die pro Fach mehr als
1 Prüfung hatten, herausfinden, können wir dies mit folgendem SQL Statement erreichen:

```sql
SELECT e.StudentNr, e.SubjectId, COUNT(*) AS Count
FROM   Exams
GROUP BY e.StudentNr, e.SubjectId
HAVING COUNT(*) > 1
```

Mit *aggregate()* funktioniert dies durch die *Pipeline* sehr einfach. Wir schreiben einfach *nach*
der Gruppierung eine Filteropation ("Stage" in der Pipeline) mit *$match*. Beachte, dass bei
*$match* kein Dollar ($) bei *count* verwendet wird, da der Key sowieso immer ein Feld ist.

```javascript
db.getCollection("exams").aggregate([
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "count" : { "$sum" : 1 } } },
  { "$match" :   { "count" : { "$gt" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$count", "_id" : 0 } }
])

[
  { studentNr: 100424, subject: 'POS', count: 2 },
  { studentNr: 100172, subject: 'D', count: 2 },
  { studentNr: 100553, subject: 'D', count: 2 },
  // ...
]
```

## Sortieren: Die Stage *$sort*

In SQL gibt es mit *ORDER BY* die Möglichkeit, das Ergebnis zu sortieren. Wir können aufsteigende
Sortierung (das ist der Standard) und mit *DESC* eine absteigende Sortierung angeben:

```sql
SELECT e.StudentNr, e.SubjectId, COUNT(*) AS Count
FROM   Exams
GROUP BY e.StudentNr, e.SubjectId
HAVING COUNT(*) > 1
ORDER BY e.SubjectId, COUNT(*) DESC
```

Bei *aggregate()* ist für die Sortierung die Stage *$sort* zuständig. Wir übergeben jedes Feld
als Key und geben 1 für aufsteigend und -1 für absteigende Sortierung an.
*{ "$sort" : { "subject" : 1, "count" : -1 } }*

```javascript
db.getCollection("exams").aggregate([
  { "$group" : { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "count" : { "$sum" : 1 } } }, 
  { "$match" : { "count" : { "$gt" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$count", "_id" : 0 } }, 
  { "$sort" : { "subject" : 1, "count" : -1 } }
])

[
  { studentNr: 100668, subject: 'AM', count: 3 },
  { studentNr: 100088, subject: 'AM', count: 3 },
  { studentNr: 100129, subject: 'AM', count: 3 },
  { studentNr: 100545, subject: 'AM', count: 2 },
  // ...
  { studentNr: 100172, subject: 'D', count: 2 },
  { studentNr: 100553, subject: 'D', count: 2 },  
  // ...
]
```

## *aggregate()* ohne Gruppierung

Es macht auch Sinn, die Funktion *aggregate()* ohne Gruppierung zu verwenden. Das Konzept der
Pipelines erlaubt auch andere Anwendungen:

### Sortierung

Wenn wir alle Werte einer Collection sortiert ausgeben möchten, wird ebenfalls die Funktion
*aggregate()* verwendet. Wir müssen also nicht immer Gruppieren:

```javascript
db.getCollection("exams").aggregate([
  { "$sort" : { "student.nr" : 1, "subject._id" : -1 } }
])
```

### Filterung + Sortierung

Durch das Konzept der Pipeline ist die Kombination Filtern und Sortieren auch einfach mit
*aggregate()* gelöst:

```javascript
db.getCollection("exams").aggregate([
  { "$match" : { "grade" : 5 } },
  { "$sort" :  { "student.nr" : 1, "subject._id" : -1 } }
])
```

### Limit

Wir können auch nur eine gewisse Anzahl an Dokumenten zurückgeben lassen. Die Stage *$limit*
kann die Anzahl festlegen:

```javascript
db.getCollection("exams").aggregate([
  { "$match" : { "grade" : 5 } },
  { "$sort" : { "student.nr" : 1, "subject._id" : -1 } }, 
  { "$limit" : 3 }
])
```

## *aggregate()* mit dem .NET Treiber erzeugen lassen

Wer mit LINQ schon gearbeitet hat, kennt schon die Entsprechungen der hier gezeigten Operationen:
*Where()* für die Filterung, *GroupBy()* für die Gruppierung, *Select()* für die Projektion und
*OrderBy()* bzw. *ThenBy()* für die Sortierung.

Im nachfolgenden Beispiel werden alle Studierenden zurückgegeben, die mehr als 1 Nicht genügend
in einem Fach bekommen haben:

```c#
var negativeGrades = db.GetCollection<Exam>("exams").AsQueryable()
    .Where(e => e.Grade == 5)
    // Gruppierung nach mehreren Spalten: Wir erzeugen in C# einen anonymen Typ.
    .GroupBy(e => new { StudentNr = e.Student.Nr, Subject = e.Subject.Shortname })
    // Projektion mit Select. Wir verwenden auch hier einen anonymen Typ.
    .Select(g => new
    {
        StudentNr = g.Key.StudentNr,
        Subject = g.Key.Subject,
        Count = g.Count()  // Aggregierung: Wir können wieder LINQ verwenden.
    })
    // Filterung NACH der Gruppierung.
    .Where(g => g.Count > 1)
    // Sortierung
    .OrderBy(g => g.StudentNr).ThenByDescending(g => g.Subject)
    .ToList();
```

Dieser Ausdruck erzeugt automatisch die Pipeline:

```javascript
db.getCollection("exams").aggregate([
  { "$match" : { "grade" : 5 } },
  { "$group" : { "_id" : { "StudentNr" : "$student.nr", "Subject" : "$subject._id" }, "__agg0" : { "$sum" : 1 } } },
  { "$project" : { "StudentNr" : "$_id.StudentNr", "Subject" : "$_id.Subject", "Count" : "$__agg0", "_id" : 0 } },
  { "$match" : { "Count" : { "$gt" : 1 } } },
  { "$sort" : { "StudentNr" : 1, "Subject" : -1 } }
])

[
  { StudentNr: 100011, Subject: 'POS', Count: 2 },
  { StudentNr: 100129, Subject: 'AM', Count: 2 },
  { StudentNr: 100150, Subject: 'POS', Count: 2 },
  { StudentNr: 100424, Subject: 'POS', Count: 2 },
  { StudentNr: 100484, Subject: 'DBI', Count: 2 },
  { StudentNr: 100673, Subject: 'POS', Count: 2 }
]
```

Weitere Infos stehen in der MongoDB Dokumentation im Kapitel Aggregation:
https://docs.mongodb.com/manual/aggregation/


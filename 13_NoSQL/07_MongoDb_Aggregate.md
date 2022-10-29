# Aggregation und Pipelines

## Absetzen von Befehlen in der Shell von Studio 3T

Die unten beschriebenen Funktionen können wir 1:1 in der Shell von Studio 3T einfügen:

![](studio3t_shell_2122.png)

Für Pipelines gibt es aber noch ein besonderes Fenster, das mit dem Button *Aggregate*
geöffnet werden kann. Dort kannst du GUI unterstützt Pipelines aufbauen. Es ist auch möglich,
einen *aggregate()* Aufruf aus der Shell zu kopieren und dort einzufügen.

![](studio3t_pipeline_2127.png)

Mit Hilfe des Tabs *Query Code* kann die Pipeline für die MongoDB Clients in Python, Node.js, C#,
Java, ... kopiert werden.

## Pipelines und die Funktion *aggregate()*

### Erstes Beispiel: die Gruppierung

In der SQL Ausbildung kommt man recht schnell zu dem Punkt, wo wir Daten *gruppieren* und
*projizieren* möchten. Das folgende Kommando würde die Anzahl der positiven Prüfungsnoten
(Note < 5) ermitteln:

![](aggregation_sql_statement1016.png)

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

![](aggregation_aggregate_function_0728.png)

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
     In unserem Beispiel nennen wir das erzeugte Feld *examCount*. Der Ausdruck *{ "$sum" : 1 }*
     sieht etwas komisch aus, hat aber Logik. Es wird für jedes Dokument der Wert 1 aufsummiert.
     Somit erhalten wir die Anzahl.
- **(3) Projektion:** Mit *"$project"* definieren wir, welche Werte wir zurückgeben wollen.
  Wir können auf die vorher ermittelten Werte mit dem Dollar ($) Zeichen zugreifen (*$_id*, *$examCount*).
  In der Projektion können wir auch Felder umbenennen, so wie wir es mit *"Count" : "$examCount"*
  machen.


Möchten wir die ID Spalte (also die Spalte, nach der wir gruppiert haben) nicht ausgeben,
können wir die Ausgabe bei *project* auf 0 setzen.

```javascript
db.getCollection("exams").aggregate([
  { "$match" :   { "grade" : { "$lt" : 5 } } },  
  { "$group" :   { "_id" : "$grade", "examCount" : { "$sum" : 1 } } },
  { "$project" : { "Grade" : "$_id", "Count" : "$examCount", "_id" : 0 } }
])
```

```javascript
[
  { Grade: 1, Count: 72 },
  { Grade: 2, Count: 52 },
  { Grade: 3, Count: 61 }
  { Grade: 4, Count: 66 },
]
```

> Hinweis: Kommt ein Feld, nach dem gruppiert wird, in einem Document nicht vor, wird
> eine Gruppe mit dem ID Wert *null* erzeugt.

#### Aufbau der Pipeline im MongoDB Compass

Im Programm MongoDB Compass gibt es die Möglichkeit, die Pipeline Schritt für Schritt aufzubauen.
Es werden dann die Zwischenergebnisse angezeigt:

![](aggregation_compass_1208.png)

### Gruppierung nach mehreren Feldern

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
werden, schreiben wir sie nicht in die *$project* Operation.

```javascript
db.getCollection("exams").aggregate([
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "examCount" : { "$sum" : 1 } } },
  { "$project" : { "count" : "$examCount" } }
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
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "examCount" : { "$sum" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$examCount", "_id" : 0 } }
])

[
  { studentNr: 100004, subject: 'DBI', count: 1 },
  { studentNr: 100289, subject: 'AM', count: 2 },
  { studentNr: 100596, subject: 'D', count: 1 },
  { studentNr: 100639, subject: 'AM', count: 2 },
  // ...
]
```

### Filtern nach der Gruppierung (HAVING in SQL)

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
  { "$group" :   { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "examCount" : { "$sum" : 1 } } },
  { "$match" :   { "examCount" : { "$gt" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$examCount", "_id" : 0 } }
])

[
  { studentNr: 100424, subject: 'POS', count: 2 },
  { studentNr: 100172, subject: 'D', count: 2 },
  { studentNr: 100553, subject: 'D', count: 2 },
  // ...
]
```

### Sortieren: die Stage *$sort*

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
  { "$group" : { "_id" : { "subject" : "$subject._id", "studentNr" : "$student.nr" }, "examCount" : { "$sum" : 1 } } }, 
  { "$match" : { "examCount" : { "$gt" : 1 } } },
  { "$project" : { "studentNr" : "$_id.studentNr", "subject" : "$_id.subject", "count" : "$examCount", "_id" : 0 } }, 
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


### Eine Anzahl an Datensätzen zurückgeben: die Stage *$limit*

Wir können auch nur eine gewisse Anzahl an Dokumenten zurückgeben lassen. Die Stage *$limit*
kann die Anzahl festlegen:

```javascript
db.getCollection("exams").aggregate([
  { "$match" : { "grade" : 5 } },
  { "$sort" : { "student.nr" : 1, "subject._id" : -1 } }, 
  { "$limit" : 3 }
])
```

### Felder hinzufügen: die Stage *$addFields*

In der Stage *$project* können wir sozusagen "von 0 weg" angeben, welche Felder wir in der Ausgabe
haben möchten. Oft möchte man aber alle Felder des Originaldokumentes übernehmen und eigene Felder
hinzufügen. Die Stage *$addFields* macht das möglich:

```javascript
db.getCollection("rooms").aggregate([
    {
        "$addFields": {
            "building": { "$substr": ["$_id", 0, 1] }
        }
    }
])
```

```javascript
[
  { _id: 'A2.14', capacity: 20, building: 'A' },
  { _id: 'C5.12', building: 'C' },
  { _id: 'A2.01', building: 'A' },
  // ...
]
```

### Arrays auflösen: die Stage *$unwind*

Wir möchten nun die Anzahl der Lehrenden zählen, die an einem Wochentag ihren Home Office Tag haben.
Das ist allerdings nicht direkt mit *$group* möglich, denn das Feld *homeOfficeDays* ist ein Array.
Daher gibt es eine spezielle Stage: *$unwuind*. Sie reduziert Arrays, indem für jedes Arrayelement
der Datensatz einfach wiederholt geschrieben wird. Grafisch sieht das so aus:

![](unwind_1240.png)

Der Lehrer *ABC* ist nun 2x vorhanden. Der Lehrer *DEF* fällt weg, da er keine Home Office Tage
im Array hat. Mit dieser Technik können wir nun leicht die Anzahl der Lehrenden pro Tag zählen:

```javascript
db.getCollection("teachers").aggregate([
    { "$unwind": "$homeOfficeDays" },
    { "$group": { "_id": "$homeOfficeDays", "count": { "$sum": 1 } } }
])
```

```javascript
[ { _id: 'MO', count: 5 }, { _id: 'DI', count: 1 }, { _id: 'FR', count: 7 }, { _id: 'MI', count: 7 }, { _id: 'DO', count: 5 } ]
```


### Joins - es gibt sie doch: die Stage *$lookup*

Eine Operation, die in SQL die Basis fast aller Abfragen darstellt, ist bis jetzt noch nicht
vorgekommen: der JOIN. Da wir in der NoSQL Datenbank die Daten nicht normalisiert speichern,
ist der Bedarf natürlich nur sehr gering (zumindest sollte er das sein). Wir können aber
den JOIN zum Simulieren von Unterabfragen brauchen. Es sollen alle Prüfungen (Collection exams)
ausgegeben werden., die die minimale Punkteanzahl hatten. Klassische Lösungen mit Limit funktionieren
her nicht, da mehrere Exam Dokumente diese Punktezahl aufweisen.

Die minimale Punkteanzahl lässt sich mit einer Gruppierung leicht ermitteln:

```javascript
db.getCollection("exams").aggregate([{ "$group": { "_id": null, "minPoints": { "$min": "$points" } } } ] )
```

Da wir nach der ID *null* gruppieren, wird die ganze Collection zum Ermitteln des Minimums
verwendet. Allerdings können wir - wie in SQL - nicht mehr auf das originale Exam Dokument zugreifen.
In SQL würden wir das Problem so lösen:

```sql
SELECT *
FROM Exams e
WHERE e.Points = (SELECT MIN(e2.Points) FROM Exams e2)
```

Unsere Lösung in MongoDB arbeitet sozusagen "umgekehrt". Es wird zuerst ermittelt, welcher Wert
das Minimum in der Collection ist. Danach gehen wir nochmals in die Collection *exams* und schreiben
alle Dokumente, für die der Ausdruck *minPoints = points* zutrifft, in ein eigenes Feld mit dem
Namen *exams*. Dieser Schritt erweitert sozusagen das Ergebnis um ein weiteres Feld, wo sich 
die gefundenen Datensätze als Array befinden. Die konkreten Schritte sind:

- Ermittle mit einer *$group* Stage die minimalen Punkte der Collection exams.
- Mit der $lookup Stage können wir einen Join nochmals in die *exams* Collection machen. Als
  Joinoperation geben wir *minPoints = points* an.
- Das Ergebnis ist ein Array, nämlich alle gefundenen Dokumente in der Zielcollection. Diese werden
  im Feld *exams* gespeichert.
- Mit *unwind* lösen wir das Array auf.
- Mit *$replaceRoot* geben wir nur die Prüfungen ohne die Gruppierungsfelder (*_id* und *_minPoints*)
  zurück.


```javascript
db.getCollection("exams").aggregate([
    { "$group": { "_id": null, "minPoints": { "$min": "$points" } } },
    { "$lookup": { "from": "exams", "localField": "minPoints", "foreignField": "points", "as": "exams" } },
    { "$unwind": "$exams"},
    { "$replaceRoot": { "newRoot": "$exams" } }
])
```

Es werden 2 Dokumente zurückgegeben, da es 2 Prüfungen mit 4 Punkten gibt.

```javascript
[
  {
    _id: ObjectId("635b9b0d7a7c8b8b6a743617"),
    student: { nr: 100662, firstname: 'Amina', lastname: 'Effler' },
    teacher: {/* ... */},
    currentClass: {/* ... */},
    examClass: {/* ... */},
    subject: { _id: 'D', longname: 'Deutsch' },
    dateTime: ISODate("2023-02-21T11:00:00.000Z"),
    pointsMax: 16,
    points: 4,
    grade: 5
  },
  {
    _id: ObjectId("635b9b0d7a7c8b8b6a74361f"),
    student: { nr: 100375, firstname: 'Giada', lastname: 'Schorr' },
    teacher: {/* ... */},
    currentClass: {/* ... */},
    examClass: {/* ... */},
    subject: { _id: 'AM', longname: 'Angewandte Mathematik' },
    dateTime: ISODate("2022-02-17T11:00:00.000Z"),
    pointsMax: 17,
    points: 4,
    grade: 5
  }
]
```

*$lookup* kann auch Pipelines für komplexere Vergleiche nutzen. So können wir z. B. alle Prüfungen
suchen, die bis 1 Woche vor der letzten Prüfung abgelegt wurden. Details sind auf
https://www.mongodb.com/docs/manual/reference/operator/aggregation/lookup/
in der Dokumentation verfügbar.

Ein anderer Ansatz ohne die Verwendung von *$lookup* wäre der Folgende:

- Gruppiere nach den Punkten. Es wird also eine Gruppe für alle Exams mit 4, 5, 6, ... Punkten
  erstellt.
- In der Gruppierung speichere mit *$push* die Dokumente der Gruppe. Sie sind unter *$$ROOT* verfügbar.
- Sortiere nach der Gruppierungs ID aufsteigend.
- Nimm den ersten Datensatz, das ist die Gruppe mit den niedrigsten Punkten. Danach wird das Array
  wieder mit *$unwind* aufgelöst.

```javascript
db.getCollection("exams").aggregate([
    { "$group": { "_id": "$points", "exams": { "$push": "$$ROOT" } } },
    { "$sort": { "_id": 1 } },
    { "$limit": 1 },
    { "$unwind": "$exams"},
    { "$replaceRoot": { "newRoot": "$exams" } }
])
```

### Verwenden von JavaScript Funktionen: der Operator *$function*

Wir möchten nun das Alter von Studierenden am Stichtag 1.9.2022 berechnen und die 3 Jüngsten ausgeben.
In Javascript gibt die Funktion *Date.parse(string)* die Anzahl der Millisekunden seit dem
1.1.1970 0h UTC zurück<sup>https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/parse</sup>.

1 Tag hat 86400 Sekunden, also 86400000 Millisekunden. 1 Jahr hat im Mittel 365.25 Tage (alle 4 Jahre
ist ein Schaltjahr). Daher kann der folgende Ausdruck das Alter in Jahren mit Kommastellen berechnen:

```javascript
(Date.parse('2022-09-01') - Date.parse(dateOfBirth)) / 86_400_000 / 365.25
```

Nun verwenden wir diese Logik im Operator *$function*, damit wir ein Feld *age* zum Dokument des
Students hinzufügen können. Der Parameter *args* bestimmt, welche Felder der Funktion übergeben werden.
Mit Dollar ($) können wir wie gewohnt auf Felder der Pipeline zugreifen.
Hinweis: Arrow Functions können nicht verwendet werden.

```javascript
db.getCollection("students").aggregate([
    {
        "$project": {
            "name": "$name",
            "age": {
                "$function":
                {
                    "body": "function(dateOfBirth) { return (Date.parse('2022-09-01') - Date.parse(dateOfBirth)) / 86_400_000 / 365.25}",
                    "args": ["$dateOfBirth"],
                    "lang": "js"
                }
            }
        }
    },
    { "$sort": {"age": 1} },
    { "$limit": 3 },
])
```

Die Ausgabe beinhaltet nun das berechnete Alter mit Kommastellen:

```javascript
[
  {
    _id: 100541,
    name: { nr: 100541, firstname: 'Yaren', lastname: 'Voelkel' },
    age: 17.06776180698152
  },
  {
    _id: 100112,
    name: { nr: 100112, firstname: 'Edgar', lastname: 'Dietzsch' },
    age: 17.086926762491444
  },
  {
    _id: 100109,
    name: { nr: 100109, firstname: 'Juan', lastname: 'Vogt' },
    age: 17.10609171800137
  }
]
```

Das gleiche Ergebnis können wir aber auch ohne Verwendung einer JavaScript Funktion 
mit den Operatoren *$divide*, *$dateDiff* und *$toDate* erreichen. *ISODate()* erzeugt
aus einem String einen Datumswert.
 
```javascript
db.getCollection("students").aggregate([
    {
        "$project": {
            "name": "$name",
            "age":
                { "$divide": [{ "$dateDiff": { "startDate": { "$toDate": "$dateOfBirth" }, "endDate": ISODate("2022-09-01"), "unit": "day" } }, 365.25] }

        }
    },
    { "$sort": { "age": 1 } },
    { "$limit": 3 },
])
```

Das sieht zwar mühsamer aus, ist aber für die Verarbeitung besser:

> Executing JavaScript inside an aggregation expression may decrease performance. Only use the 
> $function operator if the provided pipeline operators cannot fulfill your application's needs.
> <sup>https://www.mongodb.com/docs/manual/reference/operator/aggregation/function/</sup>

Die vorigen Beispiele zeigen einen Ausschnitt der zur Verfügung stehenden Stages. Alle
Möglichkeiten sind in der MongoDB Dokumentation auf
https://www.mongodb.com/docs/manual/meta/aggregation-quick-reference/
beschrieben.

## *aggregate()* samt Pipeline mit dem .NET Treiber erzeugen lassen

### Aufbau mit *AsQueryable()*

Wer mit LINQ schon gearbeitet hat, kennt schon die Entsprechungen der hier gezeigten Operationen:
*Where()* für die Filterung, *GroupBy()* für die Gruppierung, *Select()* für die Projektion und
*OrderBy()* bzw. *ThenBy()* für die Sortierung.

Im nachfolgenden Beispiel werden alle Studierenden zurückgegeben, die mehr als 1 Nicht genügend
in einem Fach bekommen haben:

```c#
var negativeGrades = db.GetCollection<Exam>("exams")
    .AsQueryable()     // IMongoQueryable Interface für LINQ zurückgeben
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

### Aufbau ohne *AsQueryable()*

In .NET kann die Pipeline für komplexere Abfragen auch händisch aufgebaut werden. Die folgende
Abfrage berechnet das Alter am 1.9.2022 und gibt alle männlichen Studierenden aus, die zu diesem
Datum noch nicht 18 Jahre alt sind.

```c#
var result = db.GetCollection<Student>("students").Aggregate()
    // Filtern nach gender = "Male". Der Verbatim String (@") erlaubt das Escapen der Anführungs-
    // zeichen mit "". Das macht Visual Studio automatisch, wenn ein String in @"" kopiert wird.
    // Da bei Match aus einem Student wieder ein Student wird, verwenden wir den Typ Student
    // als output type.
    .AppendStage(PipelineStageDefinitionBuilder.Match<Student>(@"{""gender"": ""Male""}"))
    // Für die Stage $addFields gibt es keinen PipelineStageDefinitionBuilder. Daher
    // definieren wir sie bei AppendStage als String völlig "frei".
    // Da wir Felder hinzufügen, geben wir keinen Student mehr zurück sondern ein
    // allgemeines BsonDocument.
    // Für die Berechnung des Alters verwenden wir den Ausdruck
    //     dateDiff(toDate(dateOfBirth), toDate("2022-09-01"), "day") / 365.25
    // in der Operatorschreibweise (Operatoren $divide, $dateDiff, §toDate) um das Alter in Tagen
    // zu berechnen und es durch 365.25 zu dividieren (wir wollen ein genaueres Alter zurückliefern)
    .AppendStage<BsonDocument>(@"
        {
            ""$addFields"": {
                ""age"": {
                    ""$divide"": [
                        {
                            ""$dateDiff"": {
                                ""startDate"": {
                                    ""$toDate"": ""$dateOfBirth""
                                },
                                ""endDate"": {
                                    ""$toDate"": ""2022-09-01""
                                },
                                ""unit"": ""day""
                            }
                        },
                        365.25
                    ]
                }
            }
        }
    ")
    // Die $match Stage filtert nach allen Dokumenten mit age < 18.
    .AppendStage(PipelineStageDefinitionBuilder.Match<BsonDocument>(@"{""age"": {""$lt"": 18}}"))
    // Nun kommt die Projektion. Wir genem nur den Namen, die ID und das Alter zurück.
    .AppendStage(PipelineStageDefinitionBuilder.Project<BsonDocument>(@"
        {
            ""id"": ""$_id"",
            ""firstname"": ""$name.firstname"",
            ""lastname"": ""$name.lastname"",
            ""age"": ""$age""
        }
    "))
    // Nun wird mit der $sort Stage sortiert.
    .AppendStage(PipelineStageDefinitionBuilder.Sort<BsonDocument>(@"{""lastname"" : 1, ""firstname"": 1}"))
    .ToList()
    // Nach dem Ausführen erstellen wir ein .NET Objekt aus dem BsonDocument. Mit dem
    // Indexer können wir auf die Elemente zugreifen. Mit As... konvertieren wir in
    // den entsprechenden .NET Datentyp.
    .Select(r => new
    {
        Id = r["_id"].AsInt32,
        Firstname = r["firstname"].AsString,
        Lastname = r["lastname"].AsString,
        Age = Math.Round(r["age"].AsDouble, 2)
    })
    .ToList();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
```

Das Ergebnis ist wie folgt:

```javascript
[{"Id":100306,"Firstname":"Jeremie","Lastname":"Berends","Age":17.77},
{"Id":100117,"Firstname":"Sam","Lastname":"Bouschen","Age":17.4},
{"Id":100530,"Firstname":"Tobias","Lastname":"Bräutigam","Age":17.2},
{"Id":100112,"Firstname":"Edgar","Lastname":"Dietzsch","Age":17.09},
{"Id":100341,"Firstname":"Niels","Lastname":"Dörre","Age":17.75},
{"Id":100127,"Firstname":"Mohammad","Lastname":"Friedrich","Age":17.48},
{"Id":100338,"Firstname":"John","Lastname":"Grasse","Age":17.52},
{"Id":100737,"Firstname":"Sven","Lastname":"Hadwich","Age":17.4},
{"Id":100520,"Firstname":"Julius","Lastname":"Hohenberger","Age":17.11},
{"Id":100093,"Firstname":"Oke","Lastname":"Jasper","Age":17.43},
{"Id":100510,"Firstname":"Finn","Lastname":"Konig","Age":17.73},
{"Id":100116,"Firstname":"Domenic","Lastname":"Marquart","Age":17.68},
{"Id":100707,"Firstname":"Marcel","Lastname":"Marxen","Age":17.66},
{"Id":100294,"Firstname":"Xaver","Lastname":"Olbrich","Age":17.2},
{"Id":100132,"Firstname":"Mirac","Lastname":"Schuhaj","Age":17.79},
{"Id":100307,"Firstname":"Devin","Lastname":"Schwanitz","Age":17.13},
{"Id":100327,"Firstname":"Jakob","Lastname":"Schütz","Age":17.21},
{"Id":100106,"Firstname":"Jarno","Lastname":"Stolle","Age":17.85},
{"Id":100531,"Firstname":"David","Lastname":"Strunz","Age":17.57},
{"Id":100109,"Firstname":"Juan","Lastname":"Vogt","Age":17.11}]
```

## Die Pipeline mit dem Java Treiber erzeugen lassen

In Java können wir natürlich auch eine Aggregation Pipeline aufbauen. Der Treiber
*mongodb-driver-sync* arbeitet sehr nahe an der erzeugten Pipeline. Wir geben hier eine Liste
vom Typ *Document* zurück, da das Ergebnis kein Objekt vom Typ *Exam* ist. Deswegen verwenden
wir in der Funktion *getCollection()* auch *Document.class* als Typ.

```java
import org.bson.Document;

import com.mongodb.client.model.Accumulators;
import com.mongodb.client.model.Aggregates;
import com.mongodb.client.model.Filters;
import com.mongodb.client.model.Projections;
import com.mongodb.client.model.Sorts;
// ...
var results = db.getCollection("exams", Document.class).aggregate(
    Arrays.asList(
        Aggregates.match(Filters.eq("grade", 5)),
        Aggregates.group(
            Projections.fields(
                Filters.eq("subject", "$subject._id"),
                Filters.eq("studentNr", "$student.nr")),
            Accumulators.sum("count", 1)),
        Aggregates.project(
            Projections.fields(
                Projections.excludeId(),
                Projections.computed("studentNr", "$_id.studentNr"),
                Projections.computed("subject", "$_id.subject"),
                Projections.computed("count", "$count"))),
        Aggregates.match(Filters.gt("count", 1)),
        Aggregates.sort(Sorts.orderBy(Sorts.ascending("studentNr"), Sorts.descending("subject")))
    )
)
.into(new ArrayList<>());
results.forEach(doc -> System.out.println(doc));    
```
Die Ausgabe liefert natürlich die gleichen Daten wie unsere mit .NET erzeugte mit *AsQueryable*
erzeugte Pipeline:

```
Document{{studentNr=100011, subject=POS, count=2}}
Document{{studentNr=100129, subject=AM, count=2}}
Document{{studentNr=100150, subject=POS, count=2}}
Document{{studentNr=100424, subject=POS, count=2}}
Document{{studentNr=100484, subject=DBI, count=2}}
Document{{studentNr=100673, subject=POS, count=2}}
```

Es gibt auch einen Java Treiber für MongoDB, der Streams unterstützt. Auf 
https://www.mongodb.com/docs/drivers/reactive-streams/
befindet sich die Dokumentation.

Weitere allgemeine Infos zum großen Thema Aggregation stehen in der MongoDB Dokumentation bereit:
https://docs.mongodb.com/manual/aggregation/

## Übung

Du kannst die folgenden Abfragebeispiele auf 3 Arten lösen:

- Schreiben der aggregate Funktionen in der Shell von Studio 3T.
- Mit dem .NET Treiber.
- Mit dem Java Treiber.

Die korrekte Ausgabe ist unter jedem Beispiel. Die Sortierung muss - wenn es nicht extra
angegeben ist - nicht der Ausgabe entsprechen. 

Falls du die Aufgabe in **.NET** lösen möchtest, gehe so vor: Kopiere das Generatorprogramm im Ordner
*\13_NoSQL\ExamsDb* zuerst in einen eigenen Ordner (z. B. *AggregateExcercise*).
Ersetze danach die Datei *Program.cs* durch den folgenden Inhalt und schreibe deine Abfrage
an die Stelle von *TODO: Schreibe deine Abfrage und speichere sie in result.*.

Wenn du die Aggregate Abfragen in **Java** lösen möchtest, gibt es im
Ordner *13_NoSQL/examsDbClient_Java* ein Demoprogramm mit allen Modelklassen für den Zugriff
auf die Datenbank. Kopiere die Übungsbeispiele als Kommentar und gib die zurückgegebenen
Daten wie im Punkt *Die Pipeline mit dem Java Treiber erzeugen lassen* gezeigt aus.

**(1)** Wie viele männliche und weibliche Studierende gibt es im Jahr 2022 in der Collection
*students*? Verwende *currentClass.term.year* für die Filterung des Jahres.

```javascript
[{"Gender":"Female","Count":249},{"Gender":"Male","Count":228}]
```

**(2)** Wie viele Klassen gibt es pro Jahr (Feld *term.year*) und Abteilung (Feld *department*)
in der Collection *classes*?

```javascript
[{"Year":2019,"Department":"AIF","Count":10},{"Year":2019,"Department":"BIF","Count":14},
{"Year":2019,"Department":"CIF","Count":12},{"Year":2019,"Department":"KIF","Count":8},
{"Year":2020,"Department":"AIF","Count":10},{"Year":2020,"Department":"BIF","Count":14},
{"Year":2020,"Department":"CIF","Count":12},{"Year":2020,"Department":"KIF","Count":8},
{"Year":2021,"Department":"AIF","Count":10},{"Year":2021,"Department":"BIF","Count":14},
{"Year":2021,"Department":"CIF","Count":12},{"Year":2021,"Department":"KIF","Count":8},
{"Year":2022,"Department":"AIF","Count":10},{"Year":2022,"Department":"BIF","Count":14},
{"Year":2022,"Department":"CIF","Count":12},{"Year":2022,"Department":"KIF","Count":8}]
```

**(3)** Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection *tachers*?
Hinweis: Gruppiere nach der Spalte *null*. Das bedeutet soviel wie "keine Gruppierung". Der
Durchschnitt kann mit dem Operator *$avg* berechnet werden.

```javascript
3751.85
```

**(4)** Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection *tachers*,
die POS unterrichten können (also POS im Array *canTeachSubjects* haben)?

```javascript
3767.7825
```

**(5)**  Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection *tachers*
pro Anzahl der Fächer, die sie lt. *canTeachSubjects* unterrichten dürfen?
Hinweis: Der *$size* Operator ermittelt die Länge des Arrays, also die Anzahl der Fächer nach
der zu gruppieren ist.

```javascript
[{"SubjectCount":1,"AverageSalary":3951.83},
{"SubjectCount":0,"AverageSalary":3585.2733333333333333333333333},
{"SubjectCount":2,"AverageSalary":3801.79}]
```

**(6)** Wie viele Studierende sind pro Postleitzahl (Feld *address.zip*) in der Collection *students*?
Gib nur die Postleitzahl und die Anzahl aus, wenn für diese Postleitzahl mehr als 7 Studierende
eingetragen sind. Sortiere nach der Anzahl absteigend.

```javascript
[{"Zip":1490,"Count":11},{"Zip":1110,"Count":10},{"Zip":2860,"Count":9},{"Zip":1780,"Count":9},
{"Zip":1830,"Count":8},{"Zip":1440,"Count":8},{"Zip":1820,"Count":8},{"Zip":2630,"Count":8},
{"Zip":1590,"Count":8},{"Zip":2060,"Count":8}]
```

**(7)** Erstelle eine Prüfungsstatistik pro Gegenstand aus der Collection *exams*.
Auszugeben sind beste Note (Feld *grade*), die schlechteste Note, der niedrigste Wert und der höchste Wert
von points der Gruppe (Feld *points*). Verwende NICHT das Feld *pointsMax*, das Feld gibt nur die
zu erreichende Punktezahl für die Prüfung an.

```javascript
[{"Subject":"AM","MinGrade":1,"MaxGrade":5,"MinPoints":4,"MaxPoints":46},
{"Subject":"D","MinGrade":1,"MaxGrade":5,"MinPoints":4,"MaxPoints":46},
{"Subject":"DBI","MinGrade":1,"MaxGrade":5,"MinPoints":5,"MaxPoints":48},
{"Subject":"POS","MinGrade":1,"MaxGrade":5,"MinPoints":5,"MaxPoints":44}]
```

**(8)** Wie viel Prozent werden im Durchschnitt auf die Prüfungen pro Fach in der Collection *exams*
erreicht? Der Wert berechnet sich mit 100 x *points* / *pointsMax*.
Hinweis: Berechne zuerst den Prozentwert für jedes Exam Dokument und füge das Feld über die Pipeline hinzu.

```javascript
[{"Subject":"AM","Percent":62.71131017824474},
{"Subject":"D","Percent":59.132196933663955},
{"Subject":"DBI","Percent":61.694989665028125},
{"Subject":"POS","Percent":61.50315133350603}]
```

**(9)** Wie viele Prüfungen hatte jeder Lehrende in der Abteilung KIF (*examClass.department*) in der Collection
*exams* im Jahr 2021 (*dateTime* ist im Jahr 2021)? Sortiere nach dem Shortname des Lehrers.
Hinweis: Verwende für die Filterung der Prüfungen des Jahres 2021 die Vergleiche
*dateTime >= ISODate("2021-01-01")* und *dateTime < ISODate("2022-01-01")*.

```javascript
[{"TeacherShortname":"DAU","Count":1},{"TeacherShortname":"ERH","Count":1},{"TeacherShortname":"GIL","Count":1},
{"TeacherShortname":"GRE","Count":2},{"TeacherShortname":"HAR","Count":1},{"TeacherShortname":"HER","Count":1},
{"TeacherShortname":"KAL","Count":1},{"TeacherShortname":"KUR","Count":3},{"TeacherShortname":"LEW","Count":1},
{"TeacherShortname":"MÖR","Count":1},{"TeacherShortname":"NOR","Count":1},{"TeacherShortname":"PFL","Count":1},
{"TeacherShortname":"PRE","Count":3},{"TeacherShortname":"ROD","Count":2},{"TeacherShortname":"ROS","Count":4},
{"TeacherShortname":"SAC","Count":3},{"TeacherShortname":"SCH","Count":1},{"TeacherShortname":"STE","Count":1},
{"TeacherShortname":"THR","Count":1},{"TeacherShortname":"ZIP","Count":1}]
```

**(10)** Wie viele Prüfungen gab es pro Kalenderjahr in der Collection *exams*?
Verwende das Datum in *dateTime*. Sortiere die Ausgabe absteigend nach der Anzahl der Prüfungen.

```javascript
[{"Year":2023,"Count":145},{"Year":2022,"Count":111},{"Year":2021,"Count":94},
{"Year":2020,"Count":39},{"Year":2024,"Count":8}]
```

**.NET Musterprogramm für die Übung**

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

        var options = new System.Text.Json.JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),  // Umlaute als Umlaut ausgeben (nicht als \uxxxx)
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

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

        // Hinweis zum Logging: Soll die generierte aggregate() Funktion ausgegeben werden, setze
        // logging in Zeile 17 auf true.

        // *****************************************************************************************
        // Muster: Anzahl der negativen Prüfungen pro Fach
        {
            PrintHeader("Muster: Anzahl der negativen Prüfungen pro Fach.");
            var result = examsDb.Exams.AsQueryable()
                .GroupBy(e => e.Subject.Shortname)
                .Select(g => new
                {
                    Subject = g.Key,
                    Count = g.Count()
                })
                .ToList();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));

        }

        // *****************************************************************************************
        // (1) Wie viele männliche und weibliche Studierende gibt es im Jahr 2022 in der Collection
        //     students? Verwende currentClass.term.year für die Filterung des Jahres.
        {
            PrintHeader("Männliche und weibliche Studierende im Jahr 2022.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }


        // *****************************************************************************************
        // (2) Wie viele Klassen gibt es pro Jahr (Feld term.year) und Abteilung (Feld department)
        //     in der Collection classes?
        {
            PrintHeader("Klassen pro Jahr und Abteilung.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }


        // *****************************************************************************************
        // (3) Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection teachers?
        //     Hinweis: Gruppiere nach der Spalte null. Das bedeutet soviel wie "keine Gruppierung".
        //     Der Durchschnitt kann mit der Funktion Average() in LINQ berechnet werden.
        {
            PrintHeader("Durchschnittliches Gehalt aller Lehrenden.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }

        // *****************************************************************************************
        // (4) Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection teachers,
        //     die POS unterrichten können (also POS im Array canTeachSubjects haben)?
        //     Du kannst Any() in LINQ verwenden, um festzustellen, ob ein Wert in einem Array ist.
        {
            PrintHeader("Durchschnittliches Gehalt aller POS Lehrenden.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }

        // *****************************************************************************************
        // (5) Was ist das durchschnittliche Gehalt aller Lehrenden in der Collection teachers pro
        //     Anzahl der Fächer, die sie lt. canTeachSubjects unterrichten dürfen.
        //     Hinweis: das Count Property ermittelt die Länge des Arrays, also die Anzahl der Fächer.
        {
            PrintHeader("Durchschnittliches Gehalt pro Anzahl der unterrichteten Fächer.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }

        // *****************************************************************************************
        // (6) Wie viele Studierende sind pro Postleitzahl (address.zip) in der Collection students?
        //     Gib nur die Postleitzahl und die Anzahl aus, wenn für diese Postleitzahl mehr als 7
        //     Studierende eingetragen sind. Sortiere nach der Anzahl absteigend.
        {
            PrintHeader("Postleitzahlen, wo mehr als 7 Studierende in der Collection sind.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }


        // *****************************************************************************************
        // (7) Erstelle eine Prüfungsstatistik pro Gegenstand aus der Collection *exams*.
        // Auszugeben sind beste Note(Feld* grade*), die schlechteste Note, der niedrigste Wert
        // und der höchste Wert von points der Gruppe(Feld points).Verwende NICHT das Feld
        // pointsMax, das Feld gibt nur die zu erreichende Punktezahl für die Prüfung an.
        {
            PrintHeader("Beste und schlechteste Note pro Gegenstand.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }

        // *****************************************************************************************
        // (8) Wie viel Prozent werden im Durchschnitt auf die Prüfungen pro Fach in der Collection
        //     exams erreicht?
        //     Hinweis: Berechne in der Average Funktion die Prozente. Multipliziere vorher mit 100.0
        //     (double Wert, da .0)
        {
            PrintHeader("Durchschnittliche Prozente auf eine Prüfung pro Gegenstand.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));

        }

        // *****************************************************************************************
        // (9) Wie viele Prüfungen hatte jeder Lehrende in der Abteilung KIF (examClass.department)
        //     in der Collection exams im Jahr 2021 (dateTime ist im Jahr 2021)? Sortiere nach dem
        //     Shortname des Lehrers.
        //     Hinweis: Verwende für die Filterung der Prüfungen des Jahres 2021
        //     die Vergleiche >= new DateTime(2021,1,1) bzw. < new DateTime(2022, 1, 1)
        {
            PrintHeader("Prüfungen im Jahr 2021 der Abteilung KIF pro Lehrenden.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
        }


        // *****************************************************************************************
        // (10) Wie viele Prüfungen gab es pro Kalenderjahr in der Collection exams? Verwende das
        //     Datum in dateTime. Sortiere die Ausgabe nach der Anzahl der Prüfungen.
        //     Hinweis: Verwende das Year Property des .NET DateTime Wertes.
        {
            PrintHeader("Prüfungen pro Jahr.");
            var result = null as object;  // TODO: Schreibe deine Abfrage und speichere sie in result.
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
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
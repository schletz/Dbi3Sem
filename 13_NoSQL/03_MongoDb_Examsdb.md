# Die Übungsdatenbank ExamsDb

Für die Speicherung von Kolloquien der Ausbildungen für Berufstätige (Aufbaulehrgang und Kolleg)
dient diese kleine NoSQL Datenbank. Die Datenbank wurde unter folgenden Gesichtspunkten erstelle:
- Schuljahre (Terms) sind als Semester gespeichert.
- Bei den Klassen wird das dazugehörige Schuljahr gespeichert. Somit kann z. B. die 3AAIF im
  Wintersemester 2021/22 und im Wintersemester 2022/23 existieren.
- Es wurde auf eine sinnvolle Einbettung von Objekten geachtet, damit ein entsprechender
  Index erstellt werden kann.

## Erstellen der Datenbank

Voraussetzung ist ein gestarteter MongoDb Container, wie im Kapitel
[Installation](02_Mongodb_Install.md) beschrieben. Das Programm verbindet sich mit dem User
*root* und dem Passwort *1234*.

### Verwenden des Generatorprogrammes

Klone das Repository, falls das nicht schon gemacht wurde: Erstelle in der Konsole
ein Verzeichnis für die DBI Unterlagen (z. B. C:\\DBI). Wechsle danach in dieses
Verzeichnis und Klone das Repository mit dem Befehl `git clone https://github.com/schletz/Dbi3Sem.git`.

Öffne danach im Explorer das Repository und wechsle ins Verzeichnis *13_NoSQL/ExamsDb*.
Öffne die Datei [ExamDbGenerator.csproj](ExamsDb/ExamDbGenerator.csproj)
in Visual Studio oder Rider und starte das Programm.

Falls du kein Visual Studio oder eine .NET Entwicklungsumgebung installiert hast, kann das
Programm von der Kommandozeile
**im oben genannten Verzeichnis** mit dem Befehl `dotnet run` ausgeführt werden. Es wird
die [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) benötigt, die am
Rechner installiert sein muss. Du kannst mit dem Befehl `dotnet --version` prüfen, ob du die
.NET 6 SDK besitzt.

### Alternative: Verwenden vom mongoimport in der Shell des Containers

Du kannst in Docker Desktop mit *Open Terminal* beim Container *mongodb* eine Shell öffnen. Mit
den nachfolgenden Befehlen werden die JSON Dumps von Github geladen und eingespielt.

Installiere zuerst das Dienstprogramm *wget* im Container, indem du den folgenden Befehl
in das Terminal kopierst (mit Enter danach bestätigen):

```
apt-get update && apt-get install wget
```

Danach lade damit die Dumps und erstelle die Datenbank (mit Enter danach bestätigen):

```
cd /home
for collection in terms subjects rooms classes students teachers exams
do
    wget https://raw.githubusercontent.com/schletz/Dbi3Sem/master/13_NoSQL/ExamsDb/Dump/$collection.json
    mongoimport --authenticationDatabase=admin --uri="mongodb://root:1234@localhost:27017/examsDb" --file=$collection.json --drop
    rm $collection.json
done
```

Nach dem importieren verbinde dich im Terminal des Containers mit folgendem Befehl zur
Datenbank (mit Enter bestätigen):

```
/usr/bin/mongosh mongodb://root:1234@localhost:27017
```

Nun kannst du die Indizes anlegen (mit Enter bestätigen):

```
db.getCollection("classes").createIndex({"term.year":1})
db.getCollection("students").createIndex({"currentClass.shortname":1})
db.getCollection("exams").createIndex({"currentClass._id":1})
db.getCollection("exams").createIndex({"student.nr":1})
db.getCollection("exams").createIndex({"teacher.shortname":1})
exit
```

## Das Klassendiagramm

![](ExamsDb/klassendiagramm_1008.svg)

Das Klassendiagramm zeigt die Modelklassen, die für die Serialisierung der einzelnen Dokumente
verwendet werden. Es wurde in PlantUML erstellt und hat folgende Konventionen:

- Klassen, die auch als Typ für ein *Document* verwendet werden, sind mit einem orangen *E*
  gekennzeichnet (E für entity class).
- *value objects* (Klassen mit einem C) werden nicht direkt als Document verwendet, sie werden
  aber *innerhalb* eines anderen Documents als Objekttyp verwendet. Daher läuft auch ein Pfeil
  (bedeutet Assotiation) von der entity Klasse zu dieser Klasse. Genauer handelt es sich um
  eine *Komposition*, da das value object keine Id besitzt und daher nicht alleine existieren kann.
  Das wird auch durch ein gefülltes Diamond Symbol angezeigt.
- Der Name der *navigation Properties* ist über den Pfeil der Assotiation geschrieben. So hat z. B. das
  Property *name* in der Klasse *Student* den Typ *StudentName*. Wir könenn daher über dieses
  Property auf die Instanz zugreifen ("navigieren").
- *Enumerations* werden mit einem rosafarbenen *E* gekennzeichnet. Sie werden in der Datenbank
  als String gespeichert.

## Beschreibung der Dokumente

### Document Term (Collection terms)

- **id:** ID für das Semester. Das Wintersemester hat ein W am Ende, das Sommersemester ein S.
  Wintersemester 2021/22 ist also *2021W*, das Sommersemester 2019/20 hat *2019S*.
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **year:** Jahr, in dem das Schuljahr beginnt (2019 für 2019/20).
- **termType:** Winter oder Summer. Ist eine *enumeration*, die als String gespeichert wird. Das
  ist wichtig, denn sonst würde der Zahlenwert gespeichert werden. Wird nun die enumeration im
  Programmcode vertauscht, ändert sich die Bedeutung dieses Wertes!
- **start:** Erster Tag des Semesters. Da es nur ein Datumswert ist, wird er als ISO String
  der Form *YYYY-MM-DD* in der Datenbank gespeichert.
- **end:** Tag nach dem letzten Tag des Semesters. Ein Tag ist also im Semester, wenn er *kleiner*
  als der angegebene Wert ist. Grund: Indirekt wird 0:00 bei Vergleichen angenommen. Aus 2021-02-01
  wird also 2021-02-01 um 0:00.

Beachte, dass *start* und *end* als String gespeichert werden. Wenn wir sie als Datumswert
(= Timestamp) speichern, dann speichern wir auch die Zeitzone UTC. Lädt ein Client in der
Zeitzone New York (UTC-6h) die Daten und konvertiert sie in Lokalzeit, dann ändert sich das
Datum!

> Internally, Date objects are stored as a signed 64-bit integer representing the number of
> milliseconds since the Unix epoch (Jan 1, 1970). <sup>https://www.mongodb.com/docs/manual/reference/method/Date/</sup>

Ein JSON Dump dieser Collection ist in der Datei [terms.json](ExamsDb/Dump/terms.json) abrufbar.

### Document Class (Collection classes)

Speichert die Übersicht aller Klassen für die verschiedenen Schuljahre.

- **id:** ID für die Klasse. Sie berechnet sich mit *(SemesterId)_(Klassenname)*. Die
  3AAIF im Wintersemester 2019/20 hat also die ID *2019W_3AAIF*.
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **shortname:** Der Name der Klasse ohne Schuljahr, wie sie im Stundenplan aufscheint.
- **term:** Eingebettetes Semesterobjekt vom Typ *Term*. Dadurch kann *term.Year* indiziert
  werden, um schnelle Suchen nach den Klasen eines Schuljahres zu ermöglichen.
- **department:** *AIF* für Tages-Aufbaulehrgang, *BIF* für Abend-Aufbaulehrgang, *KIF* für 
  Tages-Kolleg, *CIF* für Abend-Kolleg.
- **educationLevel:** Semester der Ausbildung (3AAIF ist im 3. Semester, 5AAIF im 5. Semester, usw.)
- **letter:** Klassenzug (B für 5BAIF, usw.)
- **classTeacher:** Eingebettetes Objekt des Klassenvorstandes (Studienkoordinator). Es wird nur
  der *TeacherName* mit *Shortname*, *Firstname* und *Lastname* eingebettet.
- **roomShortname:** ID des Stammraumes. Kann null sein (bei Wanderklassen).

#### Indizes

- Das Feld *term.year* wird indiziert, da oft Abfragen nach Klassen in einem Schuljahr (z. B. dem
  aktuellen Jahr) erwartet werden.

Ein JSON Dump dieser Collection ist in der Datei [classes.json](ExamsDb/Dump/classes.json) abrufbar.

### Document Room (Collection rooms)

Speichert die zur Verfügung stehenden Räume der Schule.

- **shortname:** ID des Raumes, wie er im Stundenplan aufscheint (z. B. C4.14).
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **capacity:** Anzahl der Sitzplätze. Kann null sein.

Ein JSON Dump dieser Collection ist in der Datei [rooms.json](ExamsDb/Dump/rooms.json) abrufbar.

### Document Subject (Collection subjects)

Speichert die unterrichteten Gegenstände der Schule.

- **shortname:** ID des Gegenstandes, wie er im Stundenplan aufscheint (z. B. POS).
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **longname:** Langbezeichnung.

Ein JSON Dump dieser Collection ist in der Datei [subjects.json](ExamsDb/Dump/subjects.json) abrufbar.

### Document Teacher (Collection teachers)

Speichert alle Lehrenden der Schule.

- **id:** ID des Lehrers, wie er im Stundenplan aufscheint (z. B. SZ). Ist immer der
  Wert von *Shortname* in *Name*.
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **name:** Objekt vom Typ *TeacherName*.  Besteht aus *shortname*, *firstname* und *lastname*.
  Die Auslagerung in ein eigenes Objekt hat den Sinn, dass z. B. bei den Prüfungen nur dieser
  Teil eingebettet werden kann.
- **homeOfficeDays:** Stringliste mit den Home Office Tagen (kein Unterricht). Mögliche
  Werte sind MO, DI, MI, DO oder FR.
- **canTeachSubjects:** Liste von Objekten vom Typ *Subject*. Gibt die Gegenstände an, die der
  Lehrer unterrichten kann. Die Liste kann auch leer sein (wenn es noch nicht eingegeben wurde).

Ein JSON Dump dieser Collection ist in der Datei [teachers.json](ExamsDb/Dump/teachers.json) abrufbar.

### Document Student (Collection students)

Speichert alle Studierenden der Schule.

- **id:** ID des Studierenden. Wird in der Schülerverwaltung generiert und ist eine 6stellige
  ganze Zahl.
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **name:** Objekt vom Typ *StudentName*.  Besteht aus *nr*, *firstname* und *lastname*.
  Die Auslagerung in ein eigenes Objekt hat den Sinn, dass z. B. bei den Prüfungen nur dieser
  Teil eingebettet werden kann.
- **currentClass:** Klasse, die der Studierende im aktuellen Schuljahr besucht. Die Datenbank
  nimmt das Sommersemester 2022/23 an. Kann null sein, wenn der Studierende nicht mehr
  an der Schule ist (also aufgehört oder maturiert hat).
- **gender:** Geschlecht. Der enum Wert wird als String gespeichert, der den Wert 
  *Female*, *Male* oder *Other* haben kann.
- **address:** Die Adresse des Studierenden. Objekt vom Typ *Address*.
  Besteht aus *street*, *streetNr*, *city* und *zip* (Postleitzahl).
- **dateOfBirth:** Das Geburtsdatum. Da es nur ein Datumswert ist, wird er als ISO String
  der Form *YYYY-MM-DD* in der Datenbank gespeichert.
- **classHistory:** Liste von Objekten vom Typ *Class*. Gibt alle Klassen an, die der
  Studierende im Lauf der Jahre an der Schule besucht hat.

Beachte, dass *dateOfBirth* als String gespeichert wird. Wenn wir sie als Datumswert
(= Timestamp) speichern, dann speichern wir auch die Zeitzone UTC. Lädt ein Client in der
Zeitzone New York (UTC-6h) die Daten und konvertiert sie in Lokalzeit, dann ändert sich das
Datum!

#### Indizes

- Das Feld *currentClass.shortname* wird indiziert, da oft Abfragen welche Studierenden gerade
  in einer bestimmten Klasse sind, erwartet werden.

Ein JSON Dump dieser Collection ist in der Datei [students.json](ExamsDb/Dump/students.json) abrufbar.

### Document Exam (Collection exams)

Speichert alle Kolloquien (Prüfungen) der Studierenden.

- **id:** Generierte ID vom Typ ObjectId (interner MongoDB ID Typ).
  Steht in der Datenbank als Feld *_id*, da es der Primärschlüssel ist.
- **student:** Objekt vom Typ *StudentName*.  Besteht aus *nr*, *firstname* und *lastname*.
  Damit nicht der ganze Student eingebettet wird, beschränken wir uns auf diese Daten.
- **teacher:** Objekt vom Typ *TeacherName*.  Besteht aus *shortname*, *firstname* und *lastname*.
  Damit nicht der ganze Teacher eingebettet wird, beschränken wir uns auf diese Daten.
- **currentClass:** Klasse, die der Studierende besucht, als er die Prüfung abgelegt hat.
  Objekt vom Typ *Class*.
- **examClass:** Klasse, wo der Studierende die negative Note bekommen hat und eine Prüfung
  darüber ablegen muss. Objekt vom Typ *Class*.
- **subject:** Gegenstand der Prüfung. Objekt vom Typ *Subject*.
- **dateTime:** Zeitstempel der Prüfung. Ist ein Datums/Zeitwert.
- **pointsMax:** Punkte, die es maximal auf die Prüfung zu erreichen gab.
- **points:** Punkte, die der Studierende auf die Prüfung erreicht hat.
- **grade:** Note, die der Studierende auf die Prüfung bekommen hat.

#### Indizes

- Das Feld *currentClass.id* wird indiziert, da oft Abfragen nach allen Prüfungskandidat(inn)en
  die gerade in einer bestimmten Klasse sind, erwartet werden.
- Das Feld *student.nr* wird indiziert, da oft Abfragen nach den Prüfungen eines bestimmten
  Studierenden erwartet werden.
- Das Feld *teacher.shortname* wird indiziert, da oft Abfragen nach allen Prüfungskandidat(inn)en
  eines bestimmten Lehrenden erwartet werden.

Ein JSON Dump dieser Collection ist in der Datei [exams.json](ExamsDb/Dump/exams.json) abrufbar.

## Anhang

### Nur als Info: Export der Datenbank

Wenn du im Kapitel [Installation von MongoDb als Docker Image](02_Mongodb_Install.md) die Option
*-v* beim *docker run* Befehl gewählt hast, wird das Verzeichnis */home* auf dein Hostsystem
umgeleitet. Mit den folgenden Befehlen im Terminal des Containers kann die Datenbank als
JSON exportiert werden (mit Enter nach dem Kopieren bestätigen):

```
cd /home
for collection in terms subjects rooms classes students teachers exams
do
    mongoexport --authenticationDatabase=admin --uri="mongodb://root:1234@localhost:27017/examsDb" --collection=$collection --out=$collection.json
done
```



### Anhang: UML Code des Klasendiagrammes

Das Klassendiagramm wurde mit PlantUML erstellt. Der Quellcode kann als Hilfe für eigene
Diagramme verwendet werden.

```
@startuml
hide empty methods
left to right direction

class Address <value object> {
    *street : String
    *streetNr : String
    *City : String
    *Zip : Integer
} 

entity Class <<(E,#FF7700) Document>> {
    *id : String <<id>>
    *shortname : String
    *term : Term
    *department : String
    *educationLevel : Integer
    *letter : String
    *classTeacher : TeacherName
    roomShortname : String
}

entity Exam <<(E,#FF7700) Document>> {
    *id : ObjectId <<id, generated>>
    *student : StudentName
    *teacher : TeacherName
    *currentClass : Class
    *examClass : Class
    *subject : Subject
    *dateTime : DateTime
    *pointsMax : Integer
    *points : Integer
    *grade : Integer
}

enum Gender {
    Female
    Male
    Other
}

entity Room <<(E,#FF7700) Document>> {
    *shortname : String <<id>>
    capacity : Integer
}

entity  Student <<(E,#FF7700) Document>> {
    *id : Integer <<id>>
    *name : StudentName
    currentClass : Class
    *gender : Gender
    *address : Address
    *dateOfBirth : DateOnly
    *classHistory : List<Class>
}

class StudentName <value object> {
    *nr : Integer
    *firstname : String
    *lastname : String
}

entity Subject <<(E,#FF7700) Document>> {
    *shortname : String <<id>>
    *longname : String
}

entity Teacher <<(E,#FF7700) Document>> {
    *id : String <<id>>
    *name : TeacherName
    salary : Decimal
    *homeOfficeDays : List<String>
    *canTeachSubjects : List<Subject>
}

class TeacherName <value object> {
    *shortname : String
    *firstname : String
    *lastname : String
}

entity Term <<(E,#FF7700) Document>> {
    *id : String <<id>>
    *year : Integer
    *termType : TermType
    *start : DateOnly
    *end : DateOnly
}


enum TermType {
    Winter
    Summer
}

Class o--> Term : > term
Class *--> TeacherName : > classTeacher

Exam *--> StudentName  : > student
Exam *--> TeacherName  : > teacher
Exam o---> Class  : > currentClass
Exam o---> Class  : > examClass

Student *--> StudentName  : > name
Student o---> Class : > currentClass
Student ---> Class : > classHistory
Student o--> Gender : > gender
Student *--> Address : > address

Teacher *--> TeacherName : > name
Teacher --> Subject : > canTeachSubjects

Term o--> TermType : > termType
@enduml
```


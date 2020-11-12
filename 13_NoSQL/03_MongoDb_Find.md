# Filtern von Collections

> - Video zum Kapitel: https://web.microsoftstream.com/video/16a1260e-228b-4cef-b45d-6ee1ed66583c (mit Schullogin abrufbar)
> - Musterprogramm: Ordner [03_MongoDb_Find](03_MongoDb_Find)

So wie in jeder anderen Datenbank ist in MongoDB das Filtern von Daten ein zentraler Punkt. Wir
haben mehrere Möglichkeiten, Daten zu filtern:

- In der Mongo Shell
- In der Applikation mittels dem MongoDB Treiber

## Filtern im .NET MongoDB Treiber

### Mit dem Builder

Der Builder generiert einen Suchfilter mit dem entsprechenden Operator. Im folgenden Beispiel
wird nach der Klasse 3CHIF gesucht:

```c#
var filter = Builders<Klasse>.Filter.Eq(k => k.Id, "3CHIF");
var found = db.GetCollection<Klasse>(nameof(Klasse))
    .Find(filter);
Console.WriteLine(found);                    // Gibt den Suchfilter aus
Console.WriteLine(found.FirstOrDefault());   // Gibt die gefundene Klasse aus
```

*Eq* ist der Operator (Equals). Er bekommt 2 Parameter: Das Feld, nach dem wir filtern möchten als
Lambda Expression. Der 2. Parameter ist der Wert, der gesucht werden soll. Die nachfolgende *Find()*
Methode bekommt diesen Suchfilter. Es wird allerdings noch nichts abgefragt. Erst mit *FirstOrDefault()*
bzw. *ToList()* werden die Daten in den Speicher geladen und können verwendet werden.

### Mit AsQueryable()

Wer schon mit LINQ gearbeitet hat, findet folgenden Zugang vertrauter:

```c#
db.GetCollection<Klasse>(nameof(Klasse))
    .AsQueryable()
    .Where(k => k.Id == "3CHIF")
    .ToList()
    .ForEach(k => Console.WriteLine(k));
```

Die Methode *AsQueryable()* liefert den Typ *IMongoQueryable* zurück, welcher die LINQ Methoden
in MongoDB Ausdrücke umwandelt. Dadurch können die gewohnten Funktionen wie *Where()*, *Select()*,
*GroupBy()*, ... verwendet werden.

Mehr Beispiele sind im Musterprogramm im Ordner *03_MongoDb_Find* in der Datei
[Program.cs](03_MongoDb_Find/Program.cs) enthalten.

## Filtern in der MongoDB Shell

Im Musterprogramm werden die generierten Suchfilter des Treibers ausgegeben. Sie können auch
direkt an die Datenbank gesendet werden. Dafür wird im bin Verzeichnis von MongoDB die Shell
mit `mongo` gestartet und folgendes eingegeben:

```text
use Stundenplan
db.getCollection("Klasse").find({ "_id" : "3CHIF" })
```

![](shell_find.png)

Auf diese Art können alle angezeigten Filter des Musterprogrammes ausgeführt werden.

# Insert und Update

> Hinweis: Erzeuge die Datenbank wie im Kapitel [Übungsdatenbank ExamsDb](03_MongoDb_Examsdb.md#erstellen-der-datenbank)
> beschrieben neu, damit vorige Inserts, Updates, etc. wieder zurückgesetzt werden. 

## Absetzen von Befehlen in der Shell von Studio 3T

Nachdem du dich mit der Datenbank *examsDb* in Studio 3T verbunden hast, kannst du die unten
beschriebenen Befehle absetzen.

![](studio3t_shell_2122.png)

## Die insertOne und insertMany Funktion (mongoshell)

Die *insertOne* und *insertMany* Funktion hat den gleichen Aufbau. Sie bekommt die Daten, die
sie einfügen soll, als Parameter. Bei *insertOne* ist dies ein Objekt, bei *insertMany* ein Array.

> ### db.getCollection(name).insertOne(document, [options])
> ### db.getCollection(name).insertMany(documents, [options])

Wird bei *insertMany()* kein Array übergeben, führt dies zu einem Fehler.

### Generierung der ID

> If the document does not specify an *_id* field, then mongod will add the *_id* field and assign a 
> unique *ObjectId()* for the document before inserting. Most drivers create an ObjectId and insert 
> the *_id* field, but the mongod will create and populate the *_id* if the driver or application
> does not.
> <sup>https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/</sup>

### Beispiel

Wir möchten nun einen neuen Raum mit der ID C5.06 und einer Kapazität von 24 Plätzen in die
Collection *rooms* einfügen. Das klingt einfach, es muss aber eine Kleinigkeit, die zu großen
Problemen führen kann, berücksichtigt werden:

> The mongo shell treats all numbers as floating-point values by default. The mongo shell provides 
> the *NumberInt()* constructor to explicitly specify 32-bit integers.

Ohne die Funktion NumberInt würde also 24 als Typ *Double* eingefügt werden. Beim Verwenden
eines C# oder Java Programmes kann dies zu einem Problem führen, da das Feld *capacity* in der
Modelklasse als *int* definiert ist. Der Treiber wird zwar den Wert kommentarlos einlesen,
eine Mischung der Datentypen sollte aber vermieden werden.

```javascript
db.getCollection("rooms").insertOne({"_id": "C5.06", "capacity": NumberInt(24)});
```
Bei *insertMany()* muss ein Array übergeben werden.

```javascript
db.getCollection("rooms").insertMany([
    { "_id": "C5.06", "capacity": NumberInt(24) },
    { "_id": "C5.07" }
])
```

Schlägt das Einfügen eines Dokumentes fehl (weil z. B. die ID schon existiert), so wird keines
der übergebenen Elemente eingefügt.

Der folgende Code würde - wenn *NumberInt()* nicht verwendet wird - eine *InvalidCastException* werfen,
da wir uns darauf verlassen dass die Werte als *Int32* gespeichert sind:

```c#
var result = db.GetCollection<Room>("rooms").Aggregate()
    .AppendStage(PipelineStageDefinitionBuilder.Match<Room>(@"{ ""capacity"": { ""$exists"": { ""capacity"": true } } }"))
    .AppendStage<BsonDocument>(@"{ ""$addFields"": { ""building"": { ""$substr"": [""$_id"", 0, 1] } } }")
    .ToEnumerable()
    .Select(r => new
    {
        Id = r["_id"].AsString,
        Building = r["building"].AsString,
        Capacity = r["capacity"].AsInt32
    })
    .ToList();
```

Das Gehalt des Lehrers (Feld *salary*) ist ein Wert vom Typ *Decimal128*. Wir verwenden daher
die Funktion *NumberDecimal()* und übergeben den Wert als String, um Rundungsfehler bei
Double Werten zu vermeiden.

```javascript
db.getCollection("teachers").insertOne({
    "_id": "EIF",
    "name": {
        "shortname": "EIF",
        "firstname": "Stefan",
        "lastname":  "Eifrig",
        "email":     "eifrig@spengergasse.at"
    },
    "gender":           "Male",
    "hoursPerWeek":     NumberInt(15),
    "salary":           NumberDecimal("4100.14"),
    "homeOfficeDays":   ["MO"],
    "canTeachSubjects": [
        {
            "_id":      "DBI",
            "longname": "Datenbanken und Informationssysteme"
        }
    ]
})
```

### Einfügen mit dem .NET Treiber

Da wir die Dokumente alle als *Modelklassen* in C# definiert haben, ist das Einfügen neuer
Daten sehr einfach. Wir erstellen einfach eine neue Instanz und fügen diese mit *InsertOne()* ein.

```c#
db.GetCollection<Room>("rooms").InsertOne(new Room(Shortname: "C5.06", Capacity: 24));
```

Bei mehreren Dokumenten können wir ein Array oder Listen (genauer: Parameter, die *IEnumerable<T>*
implementieren) einfügen:

```c#
db.GetCollection<Room>("rooms").InsertMany(new Room[]
{
    new Room(Shortname: "C5.06", Capacity: 28),
    new Room(Shortname: "C5.07")
});
```

### Einfügen mit dem Java Treiber

Auch in Java haben wir alle Dokumente als *Modelklassen* definiert. Daher ist auch hier das
Einfügen neuer Daten sehr einfach:

```java
db.getCollection("rooms", Room.class).insertOne(new Room("C5.06", 24));
```

```java
db.getCollection("rooms", Room.class).insertMany(List.of(
    new Room("C5.06", 24),
    new Room("C5.07")
));
```

## Die deleteOne und deleteMany Funktion (mongoshell)

> ### db.getCollection(name).deleteOne(filter, [options])
> ### db.getCollection(name).deleteMany(filter, [options])

Wie der Name schon sagt löscht *deleteOne()* nur ein Dokument, nämlich das erste Dokument das dem
Filter entspricht). *deleteMany()* löscht alle Dokumente, die dem Suchfilter entsprechen.
Bei Filterungen nach der ID verwenden wir daher *deleteOne()*, ansonsten *deleteMany()*.

```javascript
db.getCollection("rooms").deleteOne({ "_id": "C5.03" })
db.getCollection("rooms").deleteMany({ "capacity": { "$exists": false } })
```

### Löschen mit dem .NET Treiber

Das obere Beispiel kann mit dem .NET Treiber von MongoDB und dem Filter Builder leicht
nachgebaut werden:

```c#
var db = examsDb.Db;
db.GetCollection<Room>("rooms").DeleteOne(Builders<Room>.Filter.Eq(r => r.Shortname, "C5.03"));
db.GetCollection<Room>("rooms").DeleteMany(Builders<Room>.Filter.Not(Builders<Room>.Filter.Exists(r => r.Capacity)));
```

### Löschen mit dem Java Treiber

Auch mit dem Java Treiber ist das obere Beispiel leicht umzusetzen:

```java
db.getCollection("rooms", Room.class).deleteOne(Filters.eq("_id", "C5.03"));
db.getCollection("rooms", Room.class).deleteMany(Filters.not(Filters.exists("capacity")));
```

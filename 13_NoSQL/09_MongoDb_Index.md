# Indizes

Die hier besprochenen Inhalte gelten auch für relationale Datenbanken, da die Technik der
Indizierung das selbe Grundprinzip hat. Wenn du schon über den Index in relationalen
Datenbanken bescheid weißt, kann das Wissen hier 1:1 angewandt werden.

## Anzeigen des Ausführungsplans (explain query)

In Studio 3T (oder dem MongoDB Compass) haben wir die Möglichkeit, den *Ausführungsplan* für eine
Anfrage anzuzeigen. Er gibt die Schritte an, die die Datenbank ausführen muss, um das Ergebnis
zu liefern. Dafür wird die Anfrage zuerst ausgeführt, danach kann mittels *Explain Query*
eine grafische Darstellung abgerufen werden. Diese Pläne werden für dieses Kapitel wichtig sein.

![](studio3t_explain_1146.png)

## Warum ein Index?

Im Screenshot wird folgende Anfrage an die Datenbank gesendet:

```javascript
db.getCollection("classes").find({ "term.year": 2022})
```

Der Ausführungsplan zeigt, was die Datenbank machen muss:

- Einen *Collection Scan* (COLLSCAN). Das bedeutet, die Datenbank durchsucht die ganze Collection.
  Die Zahl über dem Pfeil zur nächsten Stufe (176) gibt an, wie viele Dokumente ausgelesen wurden.
- Der nächste Schritt ist der *Filter*. Aus den 176 Dokumenten werden 44 Dokumente ausgefiltert.

Es ist zu erwarten, dass sehr oft die Klassen eines Schuljahres für Listen, Dropdown Menüs, etc.
abgefragt werden. Je größer die Collection *classes* ist, desto länger dauert das Durchsuchen
der Collection. Das Durchsuchen hat das Laufzeitverhalten *O(n)*. Das bedeutet "Ordnung von n"
und sagt aus, dass die Zeit linear mit der Anzahl *n* steigt. Doppelt so viele Datensätze bedeuten
also doppelt so viel Zeit.

Nun legen wir einen *Index* für das Feld *term.year* an:

```javascript
db.getCollection("classes").createIndex({ "term.year": 1})
```

Der Parameter der Funktion *createIndex()* (mongoshell) ist ein Objekt mit dem Aufbau
*{field: sortorder}*. 1 steht für einen aufsteigend sortieren Index (ascending), -1 würde absteigend
sortiert (descending) sein. Bei einem Index, der nur aus 1 Feld besteht, ist dies jedoch egal.

> For a single-field index and sort operations, the sort order (i.e. ascending or descending) of
> the index key does not matter because MongoDB can traverse the index in either direction.
> <sup>https://www.mongodb.com/docs/manual/indexes/#std-label-index-types</sup>

Suchen wir erneut nach den Klassen des Jahres 2022. Der Ausführungsplan hat sich nun verändert:

![](index_singlefield_plan_1204.png)

Statt einem Collection Scan sehen wir nun einen *Index scan* (IXSCAN). Die Datenbank hat also
die Strategie geändert: Der Index gibt an, an welchen (Speicher)positionen die Daten in der
Collection zu finden sind. Dadurch müssen nur 44 Dokumente aus der Datenbank geladen werden.

Grafisch dargestellt ist der Index einfach eine Tabelle, die den Indexwert (in diesem Fall
das Jahr) *sortiert* speichert. Es wird im Index eine Referenz auf das Dokument in der Collection
gespeichert.

![](index_singlefield_graphics_1223.png)

Da der Index nach dem Wert sortiert ist, kann besonders leicht nach einem Wert gesucht werden.
Durch die Sortierung ist eine [binäre Suche](https://en.wikipedia.org/wiki/Binary_search_algorithm)
möglich. Es ist vergleichbar wie wenn ein Mensch einen Namen in einer Liste sucht. Ist die Liste
unsortiert, muss er die ganze Liste durchgehen. Ist sie allerdings sortiert, kann er zur Mitte
springen. Ist der gesuchte Namen im Alphabet vorher, muss er also in der oberen Hälfte zu finden
sein. So wird die Liste immer wieder geteilt, was viel schneller als das Durchsuchen der gesamten
Liste ist.

Auch wenn wir nicht nur nach dem Jahr suchen, kann der Index beim Lesen der Collection behilflich
sein. Wir möchten nun alle Klassen der Abteilung AIF des Jahres 2022 lesen:

```javascript
db.getCollection("classes").find({ "term.year": 2022, "department": "AIF" })
```

Der Ausführungsplan zeigt, dass hier auch der Index mit dem Jahr genutzt wird.

![](index_singlefield_plan_multiple_filter_1231.png)

- Zuerst sucht die Datenbank im Index nach allen Dokumenten im Jahr 2022. Daher müssen nur
  44 Dokumente aus der Collection *classes* gelesen werden.
- In den 44 Dokumenten wird dann nach der Abteilung AIF gesucht. Daher verlassen nur 10
  der 44 Dokumente den Filter.

Bei einem ODER nützt der Index nichts mehr, da nicht alle gelieferten Dokumente im Jahr 2022
sind. Es muss daher ein Collection Scan gemacht werden.

```javascript
db.getCollection("classes").find({ "$or": [{"term.year": 2022}, {"department": "AIF"}]})
```

## Ein Index mit mehreren Feldern (compound index)

Stellen wir fest, dass sehr häufig nach den Klassen einer Abteilung in einem Jahr gefiltert wird,
so können wir auch *beide Felder* indizieren. Damit der vorher angelegte Index nicht den
Ausführungsplan verfälscht, löschen wir mit *dropIndexes()* alle Indizes bis auf den *_id* Index.

```javascript
db.getCollection("classes").dropIndexes()
db.getCollection("classes").createIndex({ "term.year": 1, "department": 1})
```

Es wird nun eine Indextabelle aufgebaut, die zuerst nach dem Jahr (*term.year*) und dann nach der
Abteilung (*department*) sortiert ist.

![](index_multifield_graphics_1240.png)

Nun führt die Abfrage nach Jahr und Abteilung zu einem reinen Indexscan. Es werden aus dem
Index zielsicher die 10 Dokumente gefunden.

![](index_multifield_plan_1246.png)

Verwechsle nicht einen Index mit 2 Feldern mit 2 getrennten Indizes für jeweils ein Feld.
Legen wir getrennte Indizes für *term.year* und *department* an, so verwendet die Datenbank
nur den *selektiveren* Index. Das ist in unserem Fall die Abteilung, wo 40 Dokumente geladen werden.

```javascript
db.getCollection("classes").dropIndexes()
db.getCollection("classes").createIndex({ "term.year": 1 })
db.getCollection("classes").createIndex({ "department": 1 })
db.getCollection("classes").find({ "term.year": 2022, "department": "AIF" })
```

![](index_multifield_separated_plan_1249.png)

Kann uns ein Index, der aus mehreren Feldern (*term.year* und *department*) besteht, auch bei der
Abfrage nach einem einzelnen
Feld wie *term.year* unterstützen? Hier kommt es auf die *Reihenfolge der Felder* an. Stellen wir die
Ausführungspläne von 2 Abfragen gegenüber. In der ersten Abfrage wird nach dem Jahr gefiltert,
in der 2. Abfrage nach der Abteilung.

![](index_multifield_compare_plan_1249.png)

Die erste Abfrage (Filterung nach *term.year*) verwendet den Index, die 2. Abfrage (Filterung nach *department*)
jedoch nicht. Beim Blick auf die Grafik
des Index zeigt sofort die Erklärung. Der Index ist zuerst nach dem Jahr, *und dann* nach der
Abteilung sortiert. Wenn wir also nach der Abteilung filtern, haben wir keine sortierte Liste.
Bei der Filterung nach den Jahr kann der Index verwendet werden, da alle gleichen Jahre
hintereinander sind.

Der Index wird *aufsteigend sortiert* angelegt. Bei einem Index mit mehreren Feldern hat dies
Auswirkungen, wie schnell eine *sort* Operation durchgeführt werden kann. Mehr Informationen dazu
sind in der MongoDB auf 
https://www.mongodb.com/docs/manual/tutorial/equality-sort-range-rule/#std-label-esr-indexing-rule
abrufbar.

## Der unique Index

Wollen wir sicherstellen, dass es pro Jahr (*term.year*) nur eine 3AAIF (Feld *shortname*) gibt, kann ein *unique index*
angelegt werden. Wie das UNIQUE Constraint bei einer relationalen Datenbank verhindert der Index,
dass es zwei Klassen mit Shortname 3AAIF im Jahr 2022 gibt.

```javascript
db.getCollection("classes").createIndex({ "term.year": 1, "shortname": 1}, {"unique": true})
```

> You can also enforce a unique constraint on compound indexes. If you use the unique constraint
> on a compound index, then MongoDB will enforce uniqueness on the combination of the index key values.
> <sup>https://www.mongodb.com/docs/manual/core/index-unique/</sup>

Bei MongoDB gibt es sogenannte *sharded collections*. Das sind sind Collections, die auf mehrere
Instanzen verteilt sind. Hier gelten besondere Einschränkungen.
<sup>https://www.mongodb.com/docs/manual/core/sharded-cluster-requirements/#unique-indexes-in-sharded-collections</sup>

## Anlegen eines Index mit dem .NET Treiber

Mit dem MongoDB Treiber in .NET kann auch von einem Programm aus leicht ein Index angelegt werden:

```c#
// Alle Indizes der Collection bis auf den _id Index löschen.
db.GetCollection<Class>("classes").Indexes.DropAll();

// Legt einen Index für das Feld term.year in der Collection classes an.
db.GetCollection<Class>("classes").Indexes.CreateOne(
    new CreateIndexModel<Class>(Builders<Class>.IndexKeys.Ascending(c => c.Term.Year)));

// Legt einen compound index mit den Feldern term.year und department an. 
// Die Reihenfolge ist hier wichtig!
// Hinweis: CreateMany legt mehrere getrennte Indizes auf einmal an, daher nicht verwechseln!
db.GetCollection<Class>("classes").Indexes.CreateOne(
    new CreateIndexModel<Class>(
        Builders<Class>.IndexKeys.Ascending(c => c.Term.Year).Ascending(c => c.Department)));

// Einen unique Index anlegen.
db.GetCollection<Class>("classes").Indexes.CreateOne(
    new CreateIndexModel<Class>(
        Builders<Class>.IndexKeys.Ascending(c => c.Term.Year).Ascending(c => c.Shortname),
        new CreateIndexOptions() { Unique = true}));
```

## Anlegen eines Index mit dem Java Treiber

```java
// import com.mongodb.client.model.IndexOptions;
// import com.mongodb.client.model.Indexes;

// Alle Indizes der Collection bis auf den _id Index löschen.
db.getCollection("classes", SchoolClass.class).dropIndexes();

// Legt einen Index für das Feld term.year in der Collection classes an.
db.getCollection("classes", SchoolClass.class).createIndex(
    Indexes.ascending("term.year"));

// Legt einen compound index mit den Feldern term.year und department an. 
// Die Reihenfolge ist hier wichtig!    
db.getCollection("classes", SchoolClass.class).createIndex(
    Indexes.compoundIndex(Indexes.ascending("term.year"), Indexes.ascending("department")));

// Einen unique Index anlegen.
db.getCollection("classes", SchoolClass.class).createIndex(
    Indexes.compoundIndex(Indexes.ascending("term.year"), Indexes.ascending("shortname")),
    new IndexOptions().unique(true));
```

## Übung

Lege ein neues .NET Projekt mit den folgenden Befehlen an und starte die Projektdatei:

```text
md IndexDemo
cd IndexDemo
dotnet new console
start IndexDemo.csproj
```

Kopiere nun die Datei [measurements.txt.bz2](Weatherdb/measurements.txt.bz2) in den Ordner,
wo die Projektdatei ist. Ersetze danach den Inhalt der Dateien *IndexDemo.csproj* und
*Program.cs* durch den folgenden Code:

<details>
<summary>Code für die Datei <em>IndexDemo.csproj</em> anzeigen</summary>

```xml
<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net6.0</TargetFramework>
		<Nullable>enable</Nullable>
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include="SharpZipLib" Version="1.*" />
		<PackageReference Include="CsvHelper" Version="30.*" />
		<PackageReference Include="MongoDB.Driver" Version="2.*" />
	</ItemGroup>
</Project>
```
</details>

<details>
<summary>Code für die Datei <em>Program.cs</em> anzeigen</summary>

```c#
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ICSharpCode.SharpZipLib.BZip2;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

public record Measurement(
    long Id, int Station, DateTime Date, DateTime Datetime, int Year, int Month, int Day, int Hour, int Minute,
    double? Temp, double? Dewp, double? Pressure, double? Prec_amount, double? Prec_duration, double? Cloud_octas,
    double? Wind_dir, double? Wind_speed, double? Max_temp, double? Min_temp, double? Sunshine);

internal class Program
{
    private static void Main(string[] args)
    {
        var measurements = ReadMeasurements("measurements.txt.bz2");
        var db = GetDatabase();
        Console.WriteLine("Some tests WITHOUT index...");
        db.DropCollection("measurements");
        ImportData(db, measurements);
        GetStationData(db, 11082);
        GetTempBelow(db, -10);

        Console.WriteLine("Some tests WITH index...");
        db.DropCollection("measurements");

        // TODO: Anlegen eines Index über das Feld Station
        // db.GetCollection<Measurement>("measurements").Indexes....

        // TODO: Anlegen eines Index über das Feld Temp
        // db.GetCollection<Measurement>("measurements").Indexes....

        ImportData(db, measurements);
        GetStationData(db, 11082);
        GetTempBelow(db, -10);
    }

    private static IMongoDatabase GetDatabase()
    {
        var conventions = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreIfNullConvention(ignoreIfNull: true)
        };
        ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
        var settings = MongoClientSettings.FromConnectionString("mongodb://root:1234@localhost:27017");
        var client = new MongoClient(settings);
        return client.GetDatabase("weatherdata");
    }
    /// <summary>
    /// Liest die Messwerte aus einer .txt.bz2 Datei und liefert sie als Liste zurück.
    /// </summary>
    private static List<Measurement> ReadMeasurements(string filename)
    {
        using var bzStream = new BZip2InputStream(File.OpenRead("measurements.txt.bz2"));
        using var decompressed = new MemoryStream();
        bzStream.CopyTo(decompressed);
        decompressed.Position = 0;
        using var reader = new StreamReader(decompressed, new UTF8Encoding(false));

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };
        using var csv = new CsvReader(reader, csvConfig);
        return csv.GetRecords<Measurement>().ToList();
    }

    public static void ImportData(IMongoDatabase db, List<Measurement> measurements)
    {
        var sw = new Stopwatch();
        GC.Collect();
        sw.Start();
        db.GetCollection<Measurement>("measurements").InsertMany(measurements);
        sw.Stop();
        Console.WriteLine($"Inserted {measurements.Count} Records in {sw.ElapsedMilliseconds} ms");
    }
    public static List<Measurement> GetStationData(IMongoDatabase db, int stationId)
    {
        var sw = new Stopwatch();
        GC.Collect();
        sw.Start();
        var result = db.GetCollection<Measurement>("measurements").Find(Builders<Measurement>.Filter.Eq(m => m.Station, stationId)).ToList();
        sw.Stop();
        Console.WriteLine($"GetStationData: {result.Count} records read in {sw.ElapsedMilliseconds} ms");
        return result;
    }
    public static List<Measurement> GetTempBelow(IMongoDatabase db, double temp)
    {
        var sw = new Stopwatch();
        GC.Collect();
        sw.Start();
        var result = db.GetCollection<Measurement>("measurements").Find(Builders<Measurement>.Filter.Lt(m => m.Temp, temp)).ToList();
        sw.Stop();
        Console.WriteLine($"GetTempBelow {temp}°: {result.Count} records read in {sw.ElapsedMilliseconds} ms");
        return result;
    }
}
```
</details>

Das Programm liest die Datei *measurements.txt.bz2* aus und importiert alle Datensätze (rd. 466_000)
in die Datenbank *weatherdata* (Collection *measurements*). Führe das Programm mit
*dotnet run* im Release Mode (z. B. mit Optimierungen) aus:

```text
dotnet run -c Release
```

Es misst die Zeiten, die das Einfügen und Abfragen der Datensätze benötigt. Im Musterprogramm ist
noch kein Index enthalten, die Zeiten sollten daher (ungefähr) ident sein.

```text
Some tests WITHOUT index...
Inserted 465962 Records in 7037 ms
GetStationData: 122105 records read in 1603 ms
GetTempBelow -10°: 4215 records read in 332 ms

Some tests WITH index...
Inserted 465962 Records in 6809 ms
GetStationData: 122105 records read in 1486 ms
GetTempBelow -10°: 4215 records read in 325 ms
```

### Die Aufgabe

Lege an der Stelle im Sourcecode, wo *TODO: Anlegen eines Index* steht, einen Index über das
entsprechende Feld an. Führe das Programm danach nochmals aus und beobachte die Laufzeit.

- Hat sich die Zeit zum Einfügen der Datensätze verändert?
- Hat sich die Zeit von *GetStationData* verändert? Hinweis: Es gibt nur 4 unterschiedliche
  Stationen in der Datenbank. Überlege dir, was dies auf die Selektivität des Index für Auswirkungen
  hat.
- Hat sich die Zeit von *GetTempBelow* verbessert?
- Lege einen geeigneten Index an, der die Abfrage der Messwerte einer Station und Jahr (Feld *year*)
  unterstützt. Es kommen häufig auch Abfragen vor, die nur das Jahr ausfiltern. Wie muss daher
  der zusammengesetzte Index angelegt werden?
- Schreibe eine Methode *GetStationDataPerYear(IMongoDatabase db, int stationId, int year)* und
  teste sie, indem du mit und ohne dem angelegten Index die Station 11082 im Jahr 2018 abfragst.
- Schreibe eine Methode *GetDataPerYear(IMongoDatabase db, int year)* und
  teste sie, indem du mit und ohne Index die Werte des Jahres 2018 abfragst.

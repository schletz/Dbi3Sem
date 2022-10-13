# Filtern von Collections

So wie in jeder anderen Datenbank ist in MongoDB das Filtern von Daten ein zentraler Punkt. Wir
haben mehrere Möglichkeiten, Daten zu filtern:

- In der Mongo Shell
- In der Applikation mittels dem MongoDB Treiber

## Filtern im .NET MongoDB Treiber

> Das Musterprogramm mit Filterbeispielen ist im Ordner [03_MongoDb_Find](03_MongoDb_Find).
> Öffne die Datei [MongoDbDemo.csproj](03_MongoDb_Find/MongoDbDemo.csproj) in Visual Studio
> und führe das Programm bei gestarteter MongoDB aus.


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
db.Klasse.find({ "_id" : "3CHIF" })
```

![](shell_find.png)

Auf diese Art können alle angezeigten Filter des Musterprogrammes ausgeführt werden.

## Übung

In der Datei [weatherwarnings.json](weatherwarnings.json) befinden sich Daten von Wetterstationen
und ausgegebene Wetterwarnungen. Sie sollen diese Information mit Hilfe einer NoSQL Datenbank
speichern. Erstellen Sie dafür mit den folgenden Befehlen in der Konsole eine leere .NET Konsolenanwendung:

```
rd /S /Q WarningClient
md WarningClient
cd WarningClient
dotnet new console
start WarningClient.csproj

```

Ersetzen Sie den Inhalt der erzeugten Datei *WarningClient.csproj* durch die nachfolgende
Version:

```xml
<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net6.0</TargetFramework>
		<ImplicitUsings>disable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
	</PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MongoDB.Driver" Version="2.*" />
  </ItemGroup>
</Project>

```

Danach erstellen Sie einen Ordner *Model* und erstellen dort 3 Klassen, die die Informationen aus dem
betreffenden Abschnitt im JSON Dokument aufnehmen sollen:

- Klasse *Station*
- Klasse *WarnMessage*
- Klasse *Warning*

Achten Sie auf korrekte Konstruktoren für die notwendigen Felder. Ersetzen Sie nun die Datei
*Program.cs* durch die untenstehende Version. Sie verwendet Ihre erstellten Klassen *Station*,
*WarnMessage* und *Warning*, um die Daten aus der JSON Datei zu lesen.

Implementieren Sie danach die 2 Abfragebeispiele im Programmcode.

```c#
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Wichtig: Bei Copy to Output Directory muss im Solution Explorer bei stundenplan.json
//          die Option Copy Always gesetzt werden!
if (!File.Exists("weatherwarnings.json"))
{
    Console.Error.WriteLine("Datei weatherwarnings.json wurde nicht gefunden. Wurde sie mit der Option copy always ins Ausgabeverzeichnis kopiert?");
    return;
}

var jsonString = File.ReadAllText("weatherwarnings.json", System.Text.Encoding.UTF8);
var weatherwarnings = JsonDocument.Parse(jsonString).RootElement;

// Verbinden zur MongoDB und schreiben der Daten als Collection
var client = new MongoClient("mongodb://root:1234@localhost:27017");
var db = client.GetDatabase("Weatherwarnings");
db.DropCollection(nameof(Station));
db.DropCollection(nameof(WarnMessage));
db.DropCollection(nameof(Warning));

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
db.GetCollection<Station>(nameof(Station)).InsertMany(weatherwarnings.GetProperty("stations").Deserialize<Station[]>(options));
db.GetCollection<WarnMessage>(nameof(WarnMessage)).InsertMany(weatherwarnings.GetProperty("warnMessages").Deserialize<WarnMessage[]>(options));
db.GetCollection<Warning>(nameof(Warning)).InsertMany(weatherwarnings.GetProperty("warnings").Deserialize<Warning[]>(options));

// TODO: Abfragen der folgenden Informationen nach folgendem Muster:
// Welche Stationen befinden sich auf über 500m?
{
    Console.WriteLine("Stationen über 500m");
    var results = db.GetCollection<Station>(nameof(Station)).AsQueryable().Where(s => s.Height > 500).ToList();
    foreach (var station in results)
        Console.WriteLine($"{station.Id} - {station.Name} auf {station.Height}m");
}

// (1) Welche Warntexte haben Gefahrenstufe 3 (dangerLevel = 3)
{

}

// (2) Welche Warnungen galten am 8.2.2018 (Beginndatum ist <= und Endedatum ist > als dieses Datum)?
{

}

// (3) Welche Warnungen sind im März 2018 mit der Gefahrenstufe 3 gespeichert? Geben Sie das Document Warning aus.
{

}

// (4) Geben Sie wie bei Beispiel (2) die Warnungen aus, nur zeigen Sie auch den Stationsnamen an.
// Was müssen Sie tun? Ist das aus Ihrer Sicht für die Performance optimal?
{
    
}

```

### Kritische Reflexion

Betrachten Sie das 4. Abfragebeispiel (Ausgabe der Warnungen vom 8.2.2018 samt Stationsname).
Was können Sie am Design der Datenbank ändern, um dieses Beispiel besser (performanter) lösen
zu können?

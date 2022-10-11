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
*Program.cs *durch die untenstehende Version. Sie verwendet Ihre erstellten Klassen *Station*,
*WarnMessage* und *Warning*, um die Daten aus der JSON Datei zu lesen.

Implementieren Sie danach die 2 Abfragebeispiele im Programmcode.

```c#
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WarningClient.Model;

// Wichtig: Bei Copy to Output Directory muss im Solution Explorer bei stundenplan.json
//          die Option Copy Always gesetzt werden!
using var filestream = new FileStream("weatherwarnings.json", FileMode.Open, FileAccess.Read);

// Lesen der JSON Datei in die erzeugten Modelklassen.
var weatherwarnings = await JsonSerializer.DeserializeAsync<WeatherwarningsJson>(
    filestream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
if (weatherwarnings is null) { return; }
Console.WriteLine($"{weatherwarnings.Stations.Count} Stationen gelesen.");
Console.WriteLine($"{weatherwarnings.WarnMessages.Count} Warnmeldungen gelesen.");
Console.WriteLine($"{weatherwarnings.Warnings.Count} Warnungen gelesen.");

// Verbinden zur MongoDB und schreiben der Daten als Collection
var client = new MongoClient("mongodb://root:1234@localhost:27017");
var db = client.GetDatabase("Weatherwarnings");
db.DropCollection(nameof(Station));
db.DropCollection(nameof(WarnMessage));
db.DropCollection(nameof(Warning));

db.GetCollection<Station>(nameof(Station)).InsertMany(weatherwarnings.Stations);
db.GetCollection<WarnMessage>(nameof(WarnMessage)).InsertMany(weatherwarnings.WarnMessages);
db.GetCollection<Warning>(nameof(Warning)).InsertMany(weatherwarnings.Warnings);

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

class WeatherwarningsJson
{
    public List<Station> Stations { get; set; } = new();
    public List<WarnMessage> WarnMessages { get; set; } = new();
    public List<Warning> Warnings { get; set; } = new();
}
```


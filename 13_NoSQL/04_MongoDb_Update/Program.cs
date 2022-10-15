using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDbDemo.Model;


Console.BackgroundColor = ConsoleColor.White;
Console.ForegroundColor = ConsoleColor.Black;
Console.Clear();
// Verbindet zur Datenbank. Der Container muss laufen und für den User root (Passwort 1234) konfiguriert sein.
var settings = MongoClientSettings.FromConnectionString("mongodb://root:1234@localhost:27017");
settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
settings.ClusterConfigurator = cb =>
{
    cb.Subscribe<CommandStartedEvent>(e =>
    {
        if (e.Command.TryGetValue("updates", out var val))
            Console.WriteLine(val);
    });
};
var client = new MongoClient(settings);
var db = client.GetDatabase("Stundenplan");
try
{
    SeedDatabase(db);
}
catch (TimeoutException)
{
    Console.Error.WriteLine("Die Datenbank ist nicht erreichbar. Läuft der Container?");
    return;
}
catch (MongoAuthenticationException)
{
    Console.Error.WriteLine("Mit dem angegebenen Benutzer konnte keine Verbindung aufgebaut werden.");
    return;
}
Console.Clear();

// *************************************************************************************************
// Lehrer BRO ändert die Email Adresse auf bro@spengergasse.at
// Shell: db.getCollection("Lehrer").updateOne({ "_id" : "BRO" },{ "$set" : { "Email" : "bro@spengergasse.at" } } )
{
    PrintHeader("Lehrer BRO ändert die Email Adresse auf BRO@spengergasse.at");
    var result = db.GetCollection<Lehrer>(nameof(Lehrer))
        .UpdateOne(
            Builders<Lehrer>.Filter.Eq(l => l.Id, "BRO"),
            Builders<Lehrer>.Update.Set(l => l.Email, "bro@spengergasse.at"
        ));
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}
// *************************************************************************************************
// Lehrer HEN ändert die Email Adresse auf wil@spengergasse.at
// Beachte: HEN ist auch Klassenvorstand und muss daher in den Klassen
//          ebenfalls geändert werden
{
    PrintHeader("Lehrer HEN ändert die Email Adresse auf hen@spengergasse.at");
    var result = db.GetCollection<Lehrer>(nameof(Lehrer))
        .UpdateOne(
            Builders<Lehrer>.Filter.Eq(l => l.Id, "HEN"),
            Builders<Lehrer>.Update.Set(l => l.Email, "hen@spengergasse.at"
        ));
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}
// Versuch mit UpdateOne. Gefährlich, da nur der erste Eintrag aktualisiert wird!
{
    var result = db.GetCollection<Klasse>(nameof(Klasse))
        .UpdateOne(
            Builders<Klasse>.Filter.Eq(l => l.Kv.Id, "HEN"),
            Builders<Klasse>.Update.Set(l => l.Kv.Email, "hen@spengergasse.at"
        ));
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}
// Wir brauchen also UpdateMany.
{
    var result = db.GetCollection<Klasse>(nameof(Klasse))
        .UpdateMany(
            Builders<Klasse>.Filter.Eq(l => l.Kv.Id, "HEN"),
            Builders<Klasse>.Update.Set(l => l.Kv.Email, "hen@spengergasse.at"
        ));
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}
// *************************************************************************************************
// Lehrer DRE bekommt eine zusätzliche Lehrbefähigung  in E
// Siehe auch https://mongodb.github.io/mongo-csharp-driver/2.5/apidocs/html/Methods_T_MongoDB_Driver_Builders_Update.htm
{
    PrintHeader("Lehrer DRE bekommt eine zusätzliche Lehrbefähigung  in E");
    var result = db.GetCollection<Lehrer>(nameof(Lehrer))
        .UpdateOne(
            Builders<Lehrer>.Filter.Eq(l => l.Id, "DRE"),
            Builders<Lehrer>.Update.AddToSet(l => l.Lehrbefaehigungen, "E")
        );
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}

// *************************************************************************************************
// Überstunden wegen Lehrermangels: Alle Lehrer, die in POS Lehrbefähigt sind und unter
// 20 Wochenstunden unterrichten, bekommen 5 Stunden dazu.
{
    PrintHeader("Alle Lehrer, die in DBI Lehrbefähigt sind und unter 20 Wochenstunden unterrichten, bekommen 5 Stunden dazu.");
    var result = db.GetCollection<Lehrer>(nameof(Lehrer))
        .UpdateMany(
            Builders<Lehrer>.Filter.And(
                Builders<Lehrer>.Filter.AnyEq(l => l.Lehrbefaehigungen, "DBI"),
                Builders<Lehrer>.Filter.Lt(l => l.Wochenstunden, 20)),
            Builders<Lehrer>.Update.Inc(l => l.Wochenstunden, 5));
    Console.WriteLine($"{result.MatchedCount} Datensätze gefunden.");
}

// *************************************************************************************************
// Einsparung: Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger Gehalt.
// Vorsicht bei decimal Werten. Sie werden als String gespeichert, deswegen geht diese Lösung NICHT!
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 EUR weniger (Versuch mit UpdateMany)");
try
{
    db.GetCollection<Lehrer>(nameof(Lehrer))
        .UpdateMany(
            Builders<Lehrer>.Filter.Gt(l => l.Gehalt, 4000),
            Builders<Lehrer>.Update.Inc(l => l.Gehalt, -100));
}
catch (MongoWriteException e)
{
    Console.Error.WriteLine($"UpdateMany hat nicht funktioniert: {e.InnerException?.Message}.");
}

// Schlechte Lösung: Die folgende Lösung verwendet Replace und ersetzt das ganze Lehrerdokument.
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 EUR weniger (mit ReplaceOne)");
foreach (var teacher in db.GetCollection<Lehrer>(nameof(Lehrer)).AsQueryable().Where(l => l.Gehalt > 4000))
{
    teacher.Gehalt -= 100;
    db.GetCollection<Lehrer>(nameof(Lehrer)).ReplaceOne(Builders<Lehrer>.Filter.Eq(le => le.Id, teacher.Id), teacher);
}

PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 EUR weniger (mit UpdateOne)");
// Andere Möglichkeit: Update in einer Schleife.
foreach (var teacher in db.GetCollection<Lehrer>(nameof(Lehrer)).AsQueryable().Where(l => l.Gehalt > 4000))
{
    db.GetCollection<Lehrer>(nameof(Lehrer)).UpdateOne(
        Builders<Lehrer>.Filter.Eq(le => le.Id, teacher.Id),
        Builders<Lehrer>.Update.Set(le => le.Gehalt, teacher.Gehalt - 100));
}

// Besser: Verwenden von BulkWrite
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 EUR weniger (mit BulkWrite)");
var requests = db.GetCollection<Lehrer>(nameof(Lehrer))
    .AsQueryable()
    .Where(l => l.Gehalt > 4000)
    .ToList()  // <-- Wichtig, sonst gibt es eine Exception beim Bau des Eq Filters. Grund: l.Id wird "von außen" verwendet.
    .Select(l => new UpdateOneModel<Lehrer>(
        Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id),
        Builders<Lehrer>.Update.Set(le => le.Gehalt, l.Gehalt - 100)));
db.GetCollection<Lehrer>(nameof(Lehrer)).BulkWrite(requests);


void SeedDatabase(IMongoDatabase db)
{
    db.DropCollection(nameof(Lehrer));
    db.DropCollection(nameof(Klasse));

    Randomizer.Seed = new Random(2112);
    var faecher = new string[] { "POS", "DBI", "AM", "D", "E", "BWM" };
    // Wir generieren einige Lehrer mit dem Faker aus dem Paket Bogus.
    var lehrer = new Faker<Lehrer>("de").CustomInstantiator(f =>
    {
        var zuname = f.Name.LastName();
        var l = new Lehrer(
            id: zuname.Length < 3 ? zuname.ToUpper() : zuname.Substring(0, 3).ToUpper(),
            vorname: f.Name.FirstName(),
            zuname: zuname,
            wochenstunden: f.Random.Int(10, 25),
            email: $"{zuname}@spengergasse.at".OrDefault(f, 0.2f),
            gehalt: (f.Random.Int(200000, 500000) / 100M).OrNull(f, 0.5f));
        // Lehrer sind in 0 - 3 Fächern aus der Liste lehrbefähigt.
        l.Lehrbefaehigungen.AddRange(f.Random.ListItems(faecher, f.Random.Int(0, 3)));
        return l;
    })
    .Generate(100)
    .GroupBy(l => l.Id)        // Duplikate vermeiden (gleiche ID darf nicht sein)
    .Select(l => l.First())
    .ToList();

    db.GetCollection<Lehrer>(nameof(Lehrer)).InsertMany(lehrer);

    var abteilungen = new string[] { "HIF", "AIF", "BIF" };
    // Wir generieren einige Klassen.
    var klassen = new Faker<Klasse>("de").CustomInstantiator(f =>
    {
        return new Klasse(
            id: f.Random.Int(1, 5) + f.Random.String2(1, "ABCD") + f.Random.ListItem(abteilungen),
            kv: f.Random.ListItem(lehrer));
    })
    .Generate(100)
    .GroupBy(k => k.Id)       // Duplikate vermeiden (gleiche ID darf nicht sein)
    .Select(k => k.First())
    .Take(10)
    .ToList();

    db.GetCollection<Klasse>(nameof(Klasse)).InsertMany(klassen);
}

void PrintHeader(string text)
{
    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine(text);
    Console.ForegroundColor = color;
}
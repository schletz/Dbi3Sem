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

// Lehrer ABB ändert die Email Adresse auf abb@spengergasse.at
PrintHeader("Lehrer ABB ändert die Email Adresse auf abb@spengergasse.at");
db.GetCollection<Lehrer>(nameof(Lehrer))
    .UpdateOne(
        Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
        Builders<Lehrer>.Update.Set(l => l.Email, "abb@spengergasse.at"
    ));

// ************************************************************************
// Lehrer LAN ändert die Email Adresse auf lan@spengergasse.at
// Beachte: LAN ist auch Klassenvorstand und muss daher in den Klassen
//          ebenfalls geändert werden
PrintHeader("Lehrer LAN ändert die Email Adresse auf lan@spengergasse.at");
db.GetCollection<Lehrer>(nameof(Lehrer))
    .UpdateOne(
        Builders<Lehrer>.Filter.Eq(l => l.Id, "LAN"),
        Builders<Lehrer>.Update.Set(l => l.Email, "lan@spengergasse.at"
    ));
db.GetCollection<Klasse>(nameof(Klasse))
    .UpdateOne(
        Builders<Klasse>.Filter.Eq(l => l.Kv.Id, "LAN"),
        Builders<Klasse>.Update.Set(l => l.Kv.Email, "lan@spengergasse.at"
    ));

// Lehrer ABB bekommt eine zusätzliche Lehrbefähigung  in E
// Siehe auch https://mongodb.github.io/mongo-csharp-driver/2.5/apidocs/html/Methods_T_MongoDB_Driver_Builders_Update.htm
PrintHeader("Lehrer ABB bekommt eine zusätzliche Lehrbefähigung  in E");
db.GetCollection<Lehrer>(nameof(Lehrer))
    .UpdateOne(
        Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
        Builders<Lehrer>.Update.AddToSet(l => l.Lehrbefaehigungen, "E")
    );

// Einsparung: Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger
// Gehalt. Zuerst suchen wir alle Datensätze, dann werden die entsprechenden Update
// Anweisungen an die Datenbank gesendet.
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger (mit UpdateOne)");
db.GetCollection<Lehrer>(nameof(Lehrer))
    .AsQueryable()
    .Where(l => l.Gehalt > 4000)
    .ToList()
    .ForEach(l =>
    {
        db.GetCollection<Lehrer>(nameof(Lehrer)).UpdateOne(
            Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id),
            Builders<Lehrer>.Update.Set(le => le.Gehalt, l.Gehalt - 100));
    });

// Die folgende Lösung verwendet Replace und ersetzt das ganze Lehrerdokument.
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger (mit ReplaceOne)");
db.GetCollection<Lehrer>(nameof(Lehrer))
    .AsQueryable()
    .Where(l => l.Gehalt > 4000)
    .ToList()
    .ForEach(l =>
    {
        l.Gehalt = l.Gehalt - 100;
        db.GetCollection<Lehrer>(nameof(Lehrer)).ReplaceOne(
            Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id), l);
    });

// Die folgende Lösung verwendet Bulk Uperationen und ist somit schneller.
// Eine Projektion projiziert jeden Datensatz auf eine Update Operation
PrintHeader("Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger (mit BulkWrite)");
var requests = db.GetCollection<Lehrer>(nameof(Lehrer))
    .AsQueryable()
    .Where(l => l.Gehalt > 4000)
    .ToList()
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
            zuname: zuname)
        {
            Email = $"{zuname}@spengergasse.at".OrDefault(f, 0.2f),
            Gehalt = (f.Random.Int(200000, 500000) / 100M).OrNull(f, 0.5f)
        };
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
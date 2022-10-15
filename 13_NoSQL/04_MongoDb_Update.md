# Update von Dokumenten

> - Musterprogramm: Ordner [04_MongoDb_Update](04_MongoDb_Update)

```c#
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
// Lehrer HEN ändert die Email Adresse auf hen@spengergasse.at
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
// Überstunden wegen Lehrermangels: Alle Lehrer, die in DBI lehrbefähigt sind und unter
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
```
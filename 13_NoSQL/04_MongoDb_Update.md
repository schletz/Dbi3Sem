# Update von Dokumenten

> - Musterprogramm: Ordner [04_MongoDb_Update](04_MongoDb_Find)

```c#
            // Lehrer ABB ändert die Email Adresse auf abb@spengergasse.at
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .UpdateOne(
                    Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
                    Builders<Lehrer>.Update.Set(l => l.Email, "abb@spengergasse.at"
                ));

            // ************************************************************************
            // Lehrer LAN ändert die Email Adresse auf lan@spengergasse.at
            // Beachte: LAN ist auch Klassenvorstand und muss daher in den Klassen
            //          ebenfalls geändert werden
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
            db.GetCollection<Lehrer>(nameof(Lehrer))
                .UpdateOne(
                    Builders<Lehrer>.Filter.Eq(l => l.Id, "ABB"),
                    Builders<Lehrer>.Update.AddToSet(l => l.Lehrbefaehigung, "E")
                );

            // Einsparung: Alle Lehrer, die mehr als 4000 Euro verdienen, bekommen 100 € weniger
            // Gehalt. Zuerst suchen wir alle Datensätze, dann werden die entsprechenden Update
            // Anweisungen an die Datenbank gesendet.
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
            var requests = db.GetCollection<Lehrer>(nameof(Lehrer))
                .AsQueryable()
                .Where(l => l.Gehalt > 4000)
                .ToList()
                .Select(l => new UpdateOneModel<Lehrer>(
                    Builders<Lehrer>.Filter.Eq(le => le.Id, l.Id),
                    Builders<Lehrer>.Update.Set(le => le.Gehalt, l.Gehalt - 100)));
            db.GetCollection<Lehrer>(nameof(Lehrer)).BulkWrite(requests);
```
# SQL oder NoSQL?

Beim Vergleich der beiden Speichertechnologien werden oft Argumente wie Performance oder
Partitioning (Verteilen der Daten auf unterschiedlichen Systemen) hervorgebracht. Häufig stellen
aber unsere Applikationen keine großen Anforderungen an die Performance des Datenspeichers, da die
Datenmengen nicht groß sind.

Aus Sicht der Applikationsentwicklung (z. B. Ihre Diplomarbeit) führen eher die folgenden
Überlegungen zur Wahl von SQL oder NoSQL Systemen:

| Relational                                                                                                       | NoSQL                                                                                                           |
| ---------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- |
| Datenzentrierte   Sichtweise: Welche Daten habe/brauche ich? Wie werden sie in der Datenbank optimal gespeichert?                        | Applikationszentrierte   Sichtweise: Wie kann mein Programm auf die Daten optimal zugreifen?                    |
| Modellierungsprozess   unabhängig von der Software.                                                              | Modellierung erfolgt in   Verbindung mit den geforderten Programmfeatures.                                      |
| Zugriff   aus OOP Sprachen meist über einen OR Mapper, der das relationale Schema in   ein Objektmodell wandelt. | Daten werden als Objektgraphen   geladen, somit erfolgt ein direkterer Zugriff darauf.                                                                       |
| Mehrere   Programme können auf die Datenbank zugreifen.                                                          | Durch die stark   applikationszentrierte Sicht ist eine Verwendung in anderen Programmen   schwierig.           |
| Änderungen   im Schema brauchen Migrationen (ALTER TABLE).                                                       | Änderungen im Schema erfolgen im   Laufe des Betriebs, die neueren Datensätze haben einfach zusätzliche Felder. |
| Hohe   Anforderungen an die Validität der Daten (Fremdschlüssel, Constraints)                                    | Die Daten werden jedenfalls   gespeichert, Inkonsistenzen können in Kauf genommen werden.                           |
| Historisierung   von Daten ist schwierig.                                                                        | Durch Einbettung können die   Daten untrennbar mit dem Dokument verbunden werden.                               |
| Redundanzen   werden als Antipattern gesehen und sind zu vermeiden.                                              | Daten können für einen   schnelleren Zugriff mehrfach in unterschiedlichen Arten gespeichert werden.            |
| Erstellung   im Entwicklungsprozess mit Code first oder Database first.                                           | Erstellung erfolgt aus der   Applikation heraus.                                                                |

Einen guten Einstieg in das Thema ist auf Microsoft Docs unter der Adresse
https://docs.microsoft.com/de-de/dotnet/architecture/cloud-native/relational-vs-nosql-data
zu finden.

## Welche Arten von NoSQL Datenbanken gibt es?

Auf https://www.freecodecamp.org/news/nosql-databases-5f6639ed9574/ gibt es einen Überblick
über die Arten der Datenspeicherung in NoSQL Systemen.

## Welche NoSQL Datenbanken gibt es?

- [LiteDB](https://www.litedb.org/): Dokumentbasierende Datenbank ohne Server (Dateibasierend), für .NET
- [Mongo DB](https://www.mongodb.com/de): Dokumentbasierende Speicherung von Daten.
- [Google Firestore](https://firebase.google.com/products/firestore): Cloudbasierende Datenspeicherung
  von Google mit Echtzeitfeatures. Beliebt in der Mobile Entwicklung ("Serverless App").
- [Azure CosmosDB](https://azure.microsoft.com/de-de/services/cosmos-db/): NoSQL Lösung von
  Microsoft in der Cloud (Azure). Bietet ein SQL oder MongoDb Interface für die Entwicklung an.
- Reddis, Apache Cassandra, ...

Wenn Sie eines der Systeme für Ihre Diplomarbeit in Erwägung ziehen, prüfen Sie vorab:

- Gibt es einen Treiber für meine gewählte Plattform (.NET, Java, PHP, ...) in der Paketverwaltung
  wie z. B. NuGet (.NET)?
- Sind die Zugriffsmöglichkeiten aus der Programmiersprache gut dokumentiert?
- Wann wurde der Treiber zuletzt aktualisiert? Welche Version liegt vor?

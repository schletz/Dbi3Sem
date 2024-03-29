# Installation von MongoDb als Docker Image

## Vorbereitung: Docker Desktop

Lade von [docker.com](https://docs.docker.com/get-docker/) Docker Desktop für dein Betriebssystem.
In [diesem Video](https://www.youtube.com/watch?v=EfZTHVe0Z_c) wird die Installation gezeigt.
Die SQL Server Installation, die auch im Video gezeigt wird, muss natürlich nicht durchgeführt werden.

## Prüfen auf eine lokale Installation

Stelle sicher, dass nicht schon eine lokale Installation von MongoDB auf deinem Rechner läuft. Das
erkennst du daran, dass bei *Dienste* im Taskmanager der Dienst *MongoDB* aktiv ist. Deinstalliere
vorher die lokale Serverinstallation, denn sonst ist der Port 27017 belegt. Das würde nicht zu
einem Fehler beim Starten des Containers führen, allerdings kannst du dich dann nicht anmelden,
da der Zugriff dann auf den lokalen Server geleitet wird.

## Download des Docker Images

Führe nach erfolgter Docker Installation folgenden Befehl in der Konsole aus. Der Docker Container
(rd. 700 MB) wird durch die Umgebungsvariablen wie folgt konfiguriert:

- Port: 27017
- Username: root
- Passwort: 1234

Stelle vorher sicher, dass Docker Desktop läuft (Dockersymbol in der Taskleiste neben der Uhr).

```
docker run -d -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=1234 --name mongodb mongo
```

Alternativ (also *statt* dem vorigen Befehl): Wenn du das home Verzeichnis im Container z. B. auf
C:\Temp mappen möchtest, kann ein Parameter *-v* für Volume angegeben werden. So können Dateien
ausgetauscht werden, die z. B. bei *mongoexport* entstehen.

```
docker run -d -p 27017:27017 -v C:/Temp:/home -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=1234 --name mongodb mongo
```

## Importieren von Dumps in den Container

Wenn du z. B. bei einer Prüfung eine zip Datei mit einer exportierten Datenbank einspielen musst,
kannst du so vorgehen:

- Lade die Datei mit dem Dump (meist ein ZIP) z. B. in den Downloads Ordner.
- Entpacke das ZIP, sodass die *bson* Datei in diesem Ordner liegt.
- Öffne die Konsole (Eingabeaufforderung) in diesem Verzeichnis. Die Datei muss mit dem
  Befehl `dir *.bson` (Windows) oder `ls *.bson` (macOS) erscheinen.
- Führe die folgenden Befehle aus. Du musst *(filename_der_bson_datei)* natürlich an den Dateinamen der bson Datei
  und *(dbname)* an den gewünschten Datenbanknamen anpassen.

```
docker cp (filename_der_bson_datei) mongodb:/tmp/dump.bson
docker exec -it mongodb /usr/bin/mongorestore --username root --password 1234 --authenticationDatabase admin --db (dbname) /tmp/dump.bson
```

Dank an Herrn Pompe aus der 5AAIF für diese Anleitung.

### Absetzen von Befehlen in der Shell

Mit Docker Desktop kannst du mit der Option *Open in terminal* eine Shell öffnen:

![](docker_terminal_0825.png)

Gib danach die folgenden Befehle ein. Im Connection String wird davon ausgegangen, dass der
User wie in *docker run* beschrieben auf *root* mit dem Passwort *1234* gesetzt wurde:

```
/usr/bin/mongosh mongodb://root:1234@localhost:27017
```

Nun kann in der Shell direkt gearbeitet werden. Um eine Datenbank *firstTestDb* anzulegen und
ein Dokument einzufügen, werden folgende Befehle abgesetzt:

```
use firstTestDb;
db.getCollection("persons").insertOne({"firstname": "Vorname", "lastname": "Nachname"});
db.getCollection("persons").find({});
```

> **Wichtig:** Namen sind case-sensitive. Wird die Anweisung *use firsttestdb* statt *firstTestDb*
> verwendet, legt MongoDB eine zweite Datenbank an oder liefert keine Ergebnisse bei Filterabfragen!

## PlantUML und VS Code als Modellierungswerkzeug

Im Gegensatz zu relationalen Datenbanken, wo ER Diagramme als datenbankenunabhängiges Modellierungstool
verwendet werden können, gibt es für NoSQL keine einheitliche "Modellierungssprache". Da aber
schlussendlich der Datenbestand mittels Modelklassen in Java oder C# verwaltet wird (bzw. werden kann)
können Klassendiagramme verwendet werden. Solche Diagramme könenn wie folgt erzeugt werden:

1. Prüfe, ob Java installiert und im PATH eingetragen ist. Der Befehl *java -version* muss erkannt werden.
1. Installiere [Visual Studio Code](https://code.visualstudio.com). Achtung: Aktiviere beim Setup
   die Option "In den Explorer integrieren", damit Sie im Kontextmenü VS Code starten können.
1. Installiere die folgenden Extensions:
   - Markdown PDF
   - Markdown Preview Enhanced
   - PlantUML
1. Öffne die VS Code Konfiguration (*F1* - "*settings*" eingeben - "*Preferences: Open User Settings (JSON)*" wählen)
   und füge folgende Zeilen hinzu:

```javascript
    "markdown-pdf.plantumlOpenMarker": "```plantuml",
    "markdown-pdf.plantumlCloseMarker": "```"   
```

Nun steht durch die Extension *Markdown Preview Enhanced* ein Icon bereit, welches eine Vorschau mit
dem gerenderten Diagramm bietet:
![](preview_vscode.png)

## Weitere Links

- [MongoDb Community Server](https://www.mongodb.com/try/download/community)
- [NuGet: MongoDB Driver for .NET](https://www.nuget.org/packages/MongoDB.Driver/)
- [MongoDB CRUD Operations: Query Documents](https://docs.mongodb.com/manual/tutorial/query-documents/)
- [MongoDB und LINQ](https://mongodb.github.io/mongo-csharp-driver/2.11/reference/driver/crud/linq/)


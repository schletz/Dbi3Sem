# Installation von MongoDb als Docker Image

## Vorbereitung: Docker Desktop
Lade von [docker.com](https://docs.docker.com/get-docker/) Docker Desktop für dein Betriebssystem.
In [diesem Video](https://www.youtube.com/watch?v=EfZTHVe0Z_c) wird die Installation gezeigt.
Die SQL Server Installation, die auch im Video gezeigt wird, muss natürlich nicht durchgeführt werden.

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

### Verbinden mit dem MongoDB Compass

Für die Verwaltung der Datenbank gibt es die Software *MongoDB Compass*. Hier können auch Abfragen
an die Datenbank gesendet werden. Die Software kann von [mongodb.com](https://www.mongodb.com/products/compass)
geladen werden.

Im MongoDB Compass kann die Datenbank mit dem Verbindungsstring `mongodb://root:1234@localhost:27017` erreicht
werden. Natürlich muss die Datenbank vorher gestartet werden.

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


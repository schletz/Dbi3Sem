# Installation von MongoDb

## Videos

Die Videos sind auf Microsoft Stream mit einem Schulaccount abrufbar.

- Installation: https://web.microsoftstream.com/video/66e14f96-8189-4e81-9f73-766f7fc9e877
- Shell und Treiber: https://web.microsoftstream.com/video/010e51eb-81d9-4865-a683-ad61f5482032

### Beispieldatei mongod.cfg

Ist MongoDB in *C:\MongoDB* installiert, so kann die Konfiguration wie folgt aussehen:

```text
# Where and how to store data.
storage:
  dbPath: C:\MongoDB\data
  journal:
    enabled: true
# where to write logging data.
systemLog:
  destination: file
  logAppend: true
  path:  C:\MongoDB\log\mongod.log

# network interfaces
net:
  port: 27017
  bindIp: 127.0.0.1
```

Die Datei *startMongoDb.bat* wird in *C:\MongoDB* gespeichert und sieht so aus:

```text
cd C:\MongoDB\bin
mongod --config mongod.cfg
```

Die Dateien stehen auch unter [mongod.cfg](mongod.cfg) und [startMongoDb.bat](startMongoDb.bat) zum Download
bereit.

## Verbinden mit dem MongoDB Compass

Im MongoDB Compass kann die Datenbank mit dem Verbindungsstring `mongodb://127.0.0.1:27017` erreicht werden.

## PlantUML und VS Code als Modellierungswerkzeug

Im Gegensatz zu relationalen Datenbanken, wo ER Diagramme als datenbankenunabhängiges Modellierungstool
verwendet werden können, gibt es für NoSQL keine einheitliche "Modellierungssprache". Da aber
schlussendlich der Datenbestand mittels Modelklassen in Java oder C# verwaltet wird (bzw. werden kann)
können Klassendiagramme verwendet werden. Solche Diagramme könenn wie folgt erzeugt werden:

1. Installiere [Visual Studio Code](https://code.visualstudio.com)
2. Installiere die folgenden Extensions:
   - Markdown PDF
   - Markdown Preview Enhanced
   - PlantUML
3. Öffne die VS Code Konfiguration (*F1* - "*settings*" eingeben - "*Preferences: Open Settings (JSON)*" wählen)
   und füge folgende Zeilen hinzu:

```javascript
    "markdown-pdf.plantumlOpenMarker": "```plantuml\n@startuml",
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


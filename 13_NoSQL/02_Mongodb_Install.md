# Installation von MongoDb

- Lade von [mongodb.com](https://www.mongodb.com/try/download/community) die aktuellste
  Communitiy Version von MongoDb.
- Installiere das Programm in *C:\MongoDB*.
- Die Datenbank muss nicht als Netzwerkdienst installiert werden, da wir sie über eine bat
  Datei starten.

### Beispieldatei mongod.cfg

Ist MongoDB in *C:\MongoDB* installiert, führe folgende Schritte durch:
- Lade [mongod.cfg](mongod.cfg) herunter und kopiere sie in *C:\MongoDB\bin*
- Lade [startMongoDb.bat](startMongoDb.bat) und kopiere sie in *C:\MongoDB*
- Starte MongoDB durch Doppelklick auf die Datei *C:\MongoDB\startMongoDb.bat*

## Verbinden mit dem MongoDB Compass

Im MongoDB Compass kann die Datenbank mit dem Verbindungsstring `mongodb://127.0.0.1:27017` erreicht
werden. Natürlich muss die Datenbank vorher gestartet werden.

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

## Videos

Die Videos sind auf Microsoft Stream mit einem Schulaccount abrufbar.

- Installation: https://web.microsoftstream.com/video/66e14f96-8189-4e81-9f73-766f7fc9e877
- Shell und Treiber: https://web.microsoftstream.com/video/010e51eb-81d9-4865-a683-ad61f5482032

## Weitere Links

- [MongoDb Community Server](https://www.mongodb.com/try/download/community)
- [NuGet: MongoDB Driver for .NET](https://www.nuget.org/packages/MongoDB.Driver/)
- [MongoDB CRUD Operations: Query Documents](https://docs.mongodb.com/manual/tutorial/query-documents/)
- [MongoDB und LINQ](https://mongodb.github.io/mongo-csharp-driver/2.11/reference/driver/crud/linq/)


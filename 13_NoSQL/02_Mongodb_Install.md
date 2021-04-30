# Installation von MongoDb

## Videos

Die Videos sind auf Microsoft Stream mit einem Schulaccount abrufbar.

- Installation: https://web.microsoftstream.com/video/66e14f96-8189-4e81-9f73-766f7fc9e877
- Shell und Treiber: https://web.microsoftstream.com/video/010e51eb-81d9-4865-a683-ad61f5482032

Weitere Links:

- [MongoDb Community Server](https://www.mongodb.com/try/download/community)
- [NuGet: MongoDB Driver for .NET](https://www.nuget.org/packages/MongoDB.Driver/)
- [MongoDB CRUD Operations: Query Documents](https://docs.mongodb.com/manual/tutorial/query-documents/)
- [MongoDB und LINQ](https://mongodb.github.io/mongo-csharp-driver/2.11/reference/driver/crud/linq/)

Konfiguration:

Lade die Dateien [mongod.cfg](mongod.cfg) und [startMongoDb.bat](startMongoDb.bat) in das bin Verzeichnis von MongoDB und starte
die BAT Datei.

## Anlegen des Musterprojektes

```text
rd /S /Q NoSql_ExamManager
md NoSql_ExamManager
cd NoSql_ExamManager
md NoSql_ExamManager.Application
md NoSql_ExamManager.Webapp
md NoSql_ExamManager.Test
cd NoSql_ExamManager.Application
dotnet new classlib
dotnet add package MongoDB.Driver
cd ..\NoSql_ExamManager.Webapp
dotnet new blazorserver
cd ..\NoSql_ExamManager.Test
dotnet new xunit
cd ..
dotnet new sln
dotnet sln add NoSql_ExamManager.Application
dotnet sln add NoSql_ExamManager.Webapp
dotnet sln add NoSql_ExamManager.Test
start NoSql_ExamManager.sln
```

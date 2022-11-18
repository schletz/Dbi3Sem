# DBI im 5. Semester bzw. im IV. Jahrgang

## Lehrinhalte

<table>
  <thead>
    <tr>
      <th>PL/SQL (5. Semester / IV. Jahrgang)</th>
      <th>NoSQL (5. Semester / IV. Jahrgang)</th>
    </tr>
  </thead>
  <tbody>
    <tr>
    </tr>
    <tr>
      <td>
      </td>
      <td>
        <em>XML</em>
        <ul>
          <li> <a href="11_XML/README.md">XML</a></li>
        </ul>
        <em>JSON</em>
        <ul>
          <li> <a href="12_JSON/01_Intro.md">JSON Grundlagen</a></li>
          <li> <a href="12_JSON/02_Modelklassen.md">Modelklassen</a></li>
        </ul>
        <em> NoSQL (Dokumentbasierend mit MongoDB)</em>
        <ul>
          <li> <a href="13_NoSQL/01_Sql_vs_Nosql.md">SQL vs NoSQL</a></li>
          <li> <a href="13_NoSQL/02_Mongodb_Install.md">Installation von MongoDB und PlantUML in VS Code</a></li>
          <li> <a href="13_NoSQL/03_MongoDb_Examsdb.md">Unsere Musterdatenbank: Die ExamsDb</a></li>
          <li> <a href="13_NoSQL/04_Studio3T.md">Studio 3T als GUI</a></li>
          <li> <a href="13_NoSQL/05_MongoDb_Find.md">Filtern in MongoDB</a></li>
          <li> <a href="13_NoSQL/06_MongoDb_Update.md">Updates in MongoDB</a></li>
          <li> <a href="13_NoSQL/07_MongoDb_Aggregate.md">Aggregation und Pipelines</a></li>
          <li> <a href="13_NoSQL/08_MongoDb_InsertDelete.md">Insert und Delete</a></li>
          <li> <a href="13_NoSQL/09_MongoDb_Index.md">Index</a></li>
          <li> <a href="13_NoSQL/10_MongoDb_Atlas.md">MongoDB Atlas: MongoDB in der Cloud</a></li>          
          <li>
            Übungen zu Abfragen
            <ul>
              <li><a href="13_NoSQL/Uebungen/SalesDb/README.md">Die Sales Datenbank</a></li>
            </ul>
          </li>
          <li> <a href="https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/modeling-data">learn.microsoft.com: Data
              modeling in Azure Cosmos DB</a></li>
          <li> <a href="13_NoSQL/Projekt%20Pruefungsverwaltung">Praxisbeispiel: NoSQL Anwendung mit Repository
              Pattern</a></li>
          <li>
            Übungen zum Schemaentwurf
            <ul>
              <li><a href="13_NoSQL/Uebungen%20Modelling/Terminverwaltung.md">Buchungssystem mit Terminverwaltung</a>
              </li>
              <li><a href="13_NoSQL/Uebungen%20Modelling/HealthChecker.md">COVID Health Checker</a></li>
              <li><a href="13_NoSQL/Uebungen%20Modelling/Kalender.md">Kalender</a></li>
            </ul>
          </li>
        </ul>
        <em>Analytische Funktionen</em>
        <ul>
          <li><a href="02_Analytical%20Functions/README.md">Intro</a></li>
          <li><a href="02_Analytical%20Functions/01_Partitioning.md">Partition by</a></li>
          <li><a href="02_Analytical%20Functions/02_Rank.md">Rank</a></li>
          <li><a href="02_Analytical%20Functions/03_Window.md">Window</a></li>
        </ul>
      </td>
    </tr>
  </tbody>
</table>

## Installation von Oracle 21 als Docker Image

Im Kurs Dbi2Sem wird die Installation im
[Artikel zu Docker](https://github.com/schletz/Dbi2Sem/blob/master/01_OracleVM/03_Docker/README.md)
erklärt.

## Klonen des Repositories

Öffne die Konsole in einem geeigneten Verzeichnis (z. B. C:\DBI) und gib den folgenden Befehl ein:
```text
git clone https://github.com/schletz/Dbi3Sem.git
```

Um Änderungen zu laden, führe die Datei *resetGit.cmd* aus. Achtung: alle lokalen Änderungen werden
gelöscht.

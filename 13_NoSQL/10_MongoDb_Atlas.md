# MongoDB Atlas: MongoDB in der Cloud

Bis jetzt läuft unsere MongoDB Datenbank lokal in einem Docker Container. Für die Entwicklung
und zum Testen ist das auch die einfachste Möglichkeit. Wenn die Applikation in Produktion geht,
brauchen wir natürlich einen Datenbankserver, der im Netzwerk erreichbar ist.

MongoDB bietet auch einen Cloudservice an: *MongoDB Atlas*. Das Service läuft auf AWS, Azure oder
der Google Cloud. Es gibt auch einen free plan (M0). Dieser Plan hat natürlich Einschränkungen.

- You can deploy at most one M0 free cluster per Atlas project.
- M0 free clusters allow up to 100 operations per second.
- M0 free clusters and M2/M5 shared clusters can have a maximum of 100 databases and 500 
  collections total.
- M0 free clusters and M2/M5 shared clusters limit the total data transferred into or out of the
  cluster in a rolling seven-day period. The rate limits vary by cluster tier as follows:
  - M0: 10 GB in and 10 GB out per period  
- M0 free clusters and M2/M5 shared clusters don't support server-side JavaScript.
  For example, *$where* and *map-reduce* are unsupported.

<sup>Quelle: https://www.mongodb.com/docs/atlas/reference/free-shared-limitations/#operational-limitations</sup>

## Registrieren bei account.mongodb.com und Anlegen eines Clusters

Registriere dich bei https://account.mongodb.com/ und lege einen User an. Lade danach von
[der Atlas CLI Downloadseite](https://www.mongodb.com/docs/atlas/cli/stable/install-atlas-cli/)
das Programm *atlas* für dein Betriebssystem. Unter Windows verwende die ZIP Datei, öffne das
Archiv und ziehe die Datei *atlas.exe* im Verzeichnis *bin* in einen Ordner auf der Festplatte.

Gehe danach in die Konsole und in das Verzeichnis, wo sich die Datei *atlas.exe* befindet. Kopiere
nun den nachfolgenden Befehl und führe ihn aus.

```
atlas quickstart --currentIp --clusterName examsCluster --provider AZURE --region EUROPE_NORTH --username root --skipSampleData
```
<small>[Zur Befehlsreferenz auf mongodb.com](https://www.mongodb.com/docs/atlas/cli/stable/command/atlas-quickstart/)</small>

Am Ende wird eine Information ausgegeben, wie du dich zur Datenbank verbinden kannst:

```
Once you install the MongoDB Shell, connect to your database with:
$ mongosh -u root -p xxxx mongodb+srv://examscluster.xxx.mongodb.net
```

Erstelle einen Connection String in einem Editor:

1. Kopiere die im mongosh Befehl angezeigte URI (*mongodb+srv://...*).
2. Füge nach // einen String mit dem Aufbau (username):(passwort)@ ein.
3. Füge am Ende einen Schrägstrich (/) hinzu.
4. Am Ende muss die URI den Aufbau *mongodb+srv://(username):(password)@(server)/* haben.
 
Für unser Beispiel hat also der Connection String folgenden Aufbau:

```
mongodb+srv://root:xxxx@examscluster.xxx.mongodb.net/
```

Mit diesem Connection String kannst du dich in Studio 3T mit *Connect* &rarr; *New Connection*
verbinden, indem du diese URI einfach einfügst.

### Löschen des Clusters

Falls etwas schief gegangen ist, kann der User und der Cluster leicht gelöscht werden. In unserem
Beispiel haben wir den User *root* und den Cluster *examsCluster* angelegt.

```
atlas clusters delete examsCluster

atlas dbusers delete root
```

### Konfigurieren des Clusters

Logge dich auf https://account.mongodb.com/account/login mit dem User, den du bei der Registrierung
angegeben hat, ein. Du kannst hier das Kennwort für den User neu setzen oder die IP Adressen,
von denen zugegriffen werden darf, ändern. Die letzte Einstellung ist dann wichtig, wenn du
eine dynamische IP hast oder eine Applikation, die auf einem Server läuft, auf die Datenbank
zugreifen soll.

![](atlas_config_0902.png)

## Anlegen der Musterdatenbank in MongoDB Atlas

Nun wollen wir unsere *examsDb* in der Cloud anlegen. Das ist sehr einfach. Kopiere das
Generatorprogramm von *13_NoSQL/ExamsDb* (.NET) oder *13_NoSQL/examsdb-java* (Java) in ein neues
Verzeichnis. Ändere den Verbindungsstring der Methode *ExamDatabase.FromConnectionString*
in der Datei *Program.cs* oder *Main.java* auf die gerade erstellten URI. Wenn du das Programm 
ausführst, wird die Datenbank im erstellten Cluster angelegt. Natürlich dauert das etwas länger als 
die lokale Lösung mit dem Container.

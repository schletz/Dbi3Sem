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

### Anlegen mit atlas quickstart

Gehe danach in die Konsole und in das Verzeichnis, wo sich die Datei *atlas.exe* befindet. Zuerst
wird mit *atlas project* ein neues Projekt angelegt. In unserem Beispiel heißt es *examsProject*
Führe den nachfolgenden Befehl aus, um das Projekt anzulegen.

```
atlas projects create examsProject
```

Nach dem Ausführen des Befehles wird die ID des Projektes mit der Meldung `Project 'xxxxx' created.`
angezeigt. Kopiere nun den nachfolgenden Befehl und füge ihn in der Konsole ein. Bevor du ihn
ausführst, muss *(ProjectID)* durch die angezeigte ID des vorigen Befehls ersetzt werden.

```
atlas quickstart --accessListIp 0.0.0.0/0 --clusterName examsCluster --provider AZURE --region EUROPE_NORTH --username root --skipSampleData --projectId (ProjectID)
```

<sup>[Zur Befehlsreferenz auf mongodb.com](https://www.mongodb.com/docs/atlas/cli/stable/command/atlas-quickstart/)</sup>

Am Ende wird eine Information ausgegeben, wie du dich zur Datenbank verbinden kannst:

```
Once you install the MongoDB Shell, connect to your database with:
$ mongosh -u root -p xxxx mongodb+srv://examscluster.xxx.mongodb.net
```

### Erstellen des Connection Strings

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

Auf der [Konfigurationsseite](https://account.mongodb.com/account/login) kann im Menüpunkt
*Database* der Connection String ebenfalls nachgesehen werden.

### Konfigurieren des Clusters

Logge dich auf https://account.mongodb.com/account/login mit dem User, den du bei der Registrierung
angegeben hat, ein. Du kannst hier das Kennwort für den User neu setzen oder die IP Adressen,
von denen zugegriffen werden darf, ändern. Die letzte Einstellung ist dann wichtig, wenn du
eine dynamische IP hast oder eine Applikation, die auf einem Server läuft, auf die Datenbank
zugreifen soll.

![](atlas_config_0902.png)

Der Cluster kann im Menü *Databases* konfiguriert werden. Ein Klick auf den Namen öffnet
weitere Konfigurationsmöglichkeiten:

![](atlas_cluster_config_0941.png)

### Löschen des Clusters

Falls etwas schief gegangen ist, kannst du auf der
[Konfigurationsseite](https://account.mongodb.com/account/login)
den Menüpunkt *Projects* auswählen und das ganze Projekt löschen. Vorher muss der Cluster mit
*Terminate* beendet und gelöscht werden.


## Anlegen der Musterdatenbank in MongoDB Atlas

Nun wollen wir unsere *examsDb* in der Cloud anlegen. Das ist sehr einfach. Kopiere das
Generatorprogramm von *13_NoSQL/ExamsDb* (.NET) oder *13_NoSQL/examsdb-java* (Java) in ein neues
Verzeichnis. Ändere den Verbindungsstring der Methode *ExamDatabase.FromConnectionString*
in der Datei *Program.cs* oder *Main.java* auf die gerade erstellten URI. Wenn du das Programm 
ausführst, wird die Datenbank im erstellten Cluster angelegt. Natürlich dauert das etwas länger als 
die lokale Lösung mit dem Container.


## Anhang

### Alternative: mehr Kontrolle durch manuelles Anlegen

Zuerst wird mit *atlas project* ein neues Projekt angelegt. In unserem Beispiel heißt es *examsProject*

```
atlas projects create examsProject
```

Generiere nun mit einem Passwort Generator wie z. B. https://www.random.org/passwords/ ein starkes
Passwort für den root User (ca. 20 Stellen). Kopiere danach die folgenden Befehle in einen
Editor und ändere 2 Sachen:

- Statt *(Passwort)* füge das erzeugte Passwort ein.
- Statt *(Project ID)* füge die ID des Projektes ein. Sie wird nach dem Ausführen des Befehls
  *atlas projects* angezeigt.

```
atlas dbusers create atlasAdmin --username root --password (Passwort) --projectId (Project ID)

atlas accessLists create 0.0.0.0/0 --projectId (Project ID)

atlas clusters create examsCluster --type REPLICASET --members 3 --tier M0 --provider AZURE --region EUROPE_NORTH --projectId (angezeigte projectid einfügen)
```

<sup>[Zur Befehlsreferenz](https://www.mongodb.com/docs/atlas/cli/stable/command/atlas/)</sup>

Ein *sharded Cluster* kann mit dem folgenden Befehl angelegt werden. Er wird aber erst ab dem
Plan M30 unterstützt:

```
atlas clusters create examsCluster --type SHARDED --shards 5 --tier M0 --provider AZURE --region EUROPE_NORTH --projectId (angezeigte projectid einfügen)
```


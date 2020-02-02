# Zugriff auf die Oracle Datenbank mit EF Core

## Vorbereitung

Die Übungen verwenden .NET Core 3.1. Prüfen Sie nach der Installation mit folgendem Befehl, ob Sie
die Version 3.1 besitzen:

```text
dotnet --info
```

Um die neueste Version zu bekommen, gibt es 2 Möglichkeiten:

- Wenn Sie *Visual Studio 2019* verwenden, führen Sie mit *Start* - *Visual Studio Installer* ein Update
  auf die neueste Version durch. Hier wird auch das .NET Core Framework aktualisiert.
- Falls Sie mit anderen Werkzeugen (VS Code, ...) arbeiten, laden Sie von
  [dotnet.microsoft.com](https://dotnet.microsoft.com/download) die SDK Version und installieren diese.
- Für den Zugriff auf die Oracle Datenbank wird in den Übungen der SQL Editor *DBeaver* verwendet.
  Eine Anleitung für die Arbeit mit DBeaver finden Sie
  [hier auf GitHub](https://github.com/schletz/Dbi1Sem/blob/master/03_OracleSQL/00a_Dbeaver.md).


## Erstellen eines Users und befüllen der Datenbank

Als Ausgangsbasis verwenden wir die bei den analytischen Funktionen verwendete Sportfestdatenbank.
Erstellen Sie zuerst einen neuen User *Sportfest* mit folgenden Berechtigungen:

```sql
CREATE USER Sportfest IDENTIFIED BY oracle;
GRANT CONNECT, RESOURCE, CREATE VIEW TO Sportfest;
GRANT UNLIMITED TABLESPACE TO Sportfest;
```

Verbinden Sie sich nun mit diesem User in DBeaver und führen das
[SQL Skript aus dem Kapitel Analytische Funktionen](https://raw.githubusercontent.com/schletz/Dbi3Sem/master/02_Analytical%20Functions/sportfest.sql)
aus.

## Installieren der EF Core Tools

Führen Sie in der Konsole den folgenden Befehl aus. Er installiert die EF Core Tools. Durch diese
Tools können wir im nächsten Punkt die Modelklassen aus der bestehenden Datenbank generieren.

```text
dotnet tool update --global dotnet-ef
```

> **Hinweis:** Nach der Installation der ef Tools muss die Konsole neu geöffnet werden, da die *PATH*
> Variable geändert wurde.

## Erstellen einer Konsolenapplikation mit EF Core

Geben Sie nun in der Konsole in ein Verzeichnis Ihrer Wahl. Führen Sie danach die folgenden
Befehle aus. Diese Befehle funktionieren auch unter Linux oder macOS. Ihre virtuelle Maschine mit
der Oracle Datenbank muss für den letzten Befehl gestartet und bereit sein. Prüfen Sie das am Besten
vorher in DBeaver.

```text
rd /S /Q SportfestApp
md SportfestApp
cd SportfestApp
dotnet new console
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 2.2.6
dotnet add package Oracle.EntityFrameworkCore
dotnet run
dotnet ef dbcontext scaffold  "User Id=Sportfest;Password=oracle;Data Source=localhost:1521/orcl" Oracle.EntityFrameworkCore --output-dir Model --force --data-annotations
```

Die einzelnen Befehle bewirken folgendes:

- *dotnet new console* erzeugt eine leere .NET Core Konsolenanwendung. Es ist vergleichbar mit
  *Neues Projekt* - *Konsolenanwendung* in Visual Studio.
- *dotnet add package* fügt zu diesem Projekt NuGet Pakete hinzu. Es ist vergleichbar mit *NuGet* -
  *Manage Packages* in Visual Studio. Da das Oracle EFCore Paket im Moment noch nicht mit der
  Version 3 kompatibel ist, wird die neuste Version von EFCore 2 (2.2.6) eingebunden.
  Danach wird der Oracle Datenbanktreiber installiert.
- *dotnet ef dbcontext scaffold* erzeugt die Modelklassen aus der Datenbank. Dabei verweist der
  Verbindungsstring auf localhost. Falls Sie auf Ihre VM mittels IP Adresse zugreifen, ist dies durch
  die IP zu ersetzen.

Öffnen Sie nun die Datei SportfestApp.csproj in Visual Studio. In der Konsole können Sie dies mit
*start SportfestApp.csproj* am Schnellsten erledigen.

### Testen des Datenbankzugriffes

Zum Testen führen wir in unserem Programm nun eine kleine Abfrage aus, indem Sie die *Main* Methode
durch den folgenden Code ersetzen. Vergessen Sie nicht die in den Kommentar angegebenen *using*
Anweisungen einzubinden. Starten Sie das Programm einfach mit *F5* in Visual Studio oder
*dotnet run* in der Konsole.

```c#
static void Main(string[] args)
{
    using (ModelContext db = new ModelContext())    // using SportfestApp.Model oder STRG + . in VS
    {
        var schueler1a = from s in db.Schueler      // using System.Linq
                            where s.SKlasse == "1AHIF"
                            orderby s.SId
                            select new
                            {
                                Id = s.SId,
                                Zuname = s.SZuname
                            };

        Console.WriteLine("SCHÜLER DER 1AHIF");
        foreach (var s in schueler1a)
        {
            Console.WriteLine($"{s.Id} {s.Zuname}");
        }
    }
}
```

Das Programm soll folgende Ausgabe liefern:

```text
SCHÜLER DER 1AHIF
1001 Zuname1001
1002 Zuname1002
1003 Zuname1003
1004 Zuname1004
1005 Zuname1005
```

## Zugreifen auf Views

Erstellen Sie in der Datenbank folgende View und prüfen Sie das Ergebnis:

```sql
CREATE VIEW vBewerbe AS
SELECT E_Bewerb AS Bewerb, COUNT(*) AS Count
FROM Ergebnisse
GROUP BY E_Bewerb;

-- | BEWERB    | COUNT |
-- | --------- | ----- |
-- | 100m Lauf | 217   |
-- | 5km Lauf  | 197   |
-- | 400m Lauf | 199   |
SELECT * FROM vBewerbe;
```

Um nun auf unsere View zugreifen zu können, müssen folgende Schritte erledigt werden.

### Erstellen der Modelklasse

Für unsere Tabellen hat das *scaffold* Skript die Tabellendefinitionen erstellt. Bei Views
müssen wir selbst die Modelklasse erstellen. Diese Klasse verbindet das Ergebnis mit der
View mit der objektorientierten Welt in C#. Damit wir die Spalten nicht händisch abtippen müssen,
können wir über JSON Daten die Klasse erstellen. Markieren Sie dafür in DBeaver mit *STRG + A* alle
Ergebnisse im Result. Danach wählen Sie *Copy as JSON*.

![](images/json_to_class.png)

Nun erstellen Sie in Visual Studio eine neue Klasse *Bewerbe* im Ordner *Model*. Nun können Sie
in Visual Studio mit *Edit - Paste Special* aus den JSON Daten eine Klassendefinition einfügen.
Mit [json2csharp](http://json2csharp.com/) steht Ihnen auch eine online Lösung zur Verfügung, falls
Sie nicht mit Visual Studio arbeiten.

### Anpassen der Modelklasse mit Annotations

EF Core arbeitet nach dem *convention over configuration* Prinzip. Das bedeutet, dass ein bestimmtes
Standardverhalten keiner Konfiguration im Code bedarf. Das Standardverhalten lautet:

- Der Tabellenname entspricht dem Typnamen der Modelklasse
- Die Felder in der Tabelle entsprechen dem Namen der Properties in dieser Klasse

Allerdings trifft dies bei uns nicht zu, denn

- unsere View heißt vBewerbe und nicht Bewerb.
- der OR Mapper für Oracle setzt alle Objektnamen in der Datenbank als großgeschrieben um.

Dadurch ist es notwendig, die einzelnen Codeelemente mit Annotations (vergleichbar mit @ in Java)
zu versehen.

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SportfestApp.Model
{
    [Table("VBEWERBE")]     // using System.ComponentModel.DataAnnotations.Schema oder STRG + . in VS
    public class Bewerb
    {
        [Column("BEWERB")]
        public string Name { get; set; }
        [Column("COUNT")]
        public int Count { get; set; }
    }
}
```

### Anpassen des Contextes

Eine Modelklasse alleine gibt nur an, wie EF Code den Rückgabewert der Abfrage mappen soll. Damit
wir die View abfragen können, muss die Klasse *ModelContext* noch editiert werden. Der nachfolgende
Code gibt die Ergänzungen in den betreffenden Teilen der Klasse an. Der Rest bleibt unverändert.

```c#
public partial class ModelContext : DbContext
{
    // Andere Tabellen
    public virtual DbSet<Bewerb> Bewerbe { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguration der anderen Entities

        modelBuilder.Entity<Bewerb>(entity =>
        {
            entity.HasKey(e => e.Name);
        });
    }
}
```

### Verwenden der View

Nun können Sie die *Main* Methode in *Program.cs* ergänzen.

```c#
using (ModelContext db = new ModelContext())
{
    // Code aus der vorigen Übung
    Console.WriteLine("BEWERBE IN DER DATENBANK");
    var bewerbe = from b in db.Bewerbe
                    select b;
    foreach (var b in bewerbe)
    {
        Console.WriteLine($"{b.Name} hat {b.Count} Einträge.");
    }
}
```

Dieses Codestück soll folgende Ausgabe liefern:

```text
BEWERBE IN DER DATENBANK
100m Lauf hat 217 Einträge.
5km Lauf hat 197 Einträge.
400m Lauf hat 199 Einträge.
```

## Zugreifen auf Stored Procedures

Legen Sie in der Sportfestdatenbank eine Prozedur mit dem Namen *get_results* an. Diese
Prozedur liest Werte aus der Datenbank in einen Cursor. Dieser Cursor wird als*OUT* Parameter
an unsere Applikation geliefert.

```sql
CREATE OR REPLACE
PROCEDURE get_results (bewerb     IN  Ergebnisse.E_Bewerb%TYPE,
                      p_recordset OUT SYS_REFCURSOR) AS 
BEGIN 
  OPEN p_recordset FOR
    SELECT *
    FROM   Ergebnisse
    WHERE  E_Bewerb = bewerb
    ORDER BY E_Schueler;
END;
```

### Testen in der Mainmethode

Nun rufen wir die Prozedur auf. Da sie nur einen Teil der Tabelle *Ergebnisse* liefert, muss
keine neue Modelklasse definiert werden. Die Methode *FromSql()* bedeutet, dass EF Core diese
Anweisung ausführt und versucht, das Ergebnis auf die entsprechende Modelklasse zu mappen.

Da wir auch mit Parametern arbeiten, definieren wir sie zuerst mit einem Namen (*:bewerb*, ...).
Die weiteren Argumente spezifizieren dann den Typ und den Wert für unsere Parameter.

Fügen Sie in Ihre *Main* Methode in der Datei *Program.cs* folgenden Code ein.

```c#
using (ModelContext db = new ModelContext())
{
    // Code aus der vorigen Übung

    // using System.Data;
    // using Microsoft.EntityFrameworkCore
    // using Oracle.ManagedDataAccess.Client
    var results = db.Ergebnisse.FromSql("BEGIN get_results(:bewerb, :result); END;",
        new OracleParameter("bewerb", "100m Lauf"),
        new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output));

    Console.WriteLine($"{results.Count()} Bewerbe gefunden.");
}
```

### Einbauen in den Context

Die vorige Technik funktioniert zwar, ist aber - wenn die Prozedur mehrmals aufgerufen werden
soll - sperrig. Außerdem ist keine Typprüfung des Parameters *bewerb* möglich, da er als
*object* übergeben wird.

Wir wollen nun unsere Klasse *ModelContext* durch eine C# Funktion *GetResults*
erweitern, die genau den oberen Code aufruft. *FromSql()* liefert ein Ergebnis des Typs
*IQueryable&lt;T&gt;*. Diesen Rückgabewert geben wir
auch unserer Funktion. Der *=&gt;* Operator ermöglicht ab C# 7 das Einsparen des *return*
Statements und gibt automatisch das Ergebnis zurück.

```c#
// Nicht vergessen:
// using System.Data;
// using System.Linq;
// using Microsoft.EntityFrameworkCore
// using Oracle.ManagedDataAccess.Client
public partial class ModelContext : DbContext
{
    // Andere Tabellendefinitionen
    public IQueryable<Ergebnisse> GetResults(string bewerb) =>
        Ergebnisse.FromSql("BEGIN get_results(:bewerb, :result); END;",
                            new OracleParameter("bewerb", bewerb),
                            new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output));
}
```

Nun kann in der Main Methode einfach die Prozedur über die Contextklasse aufgerufen werden:

```c#
var results = db.GetResults("100m Lauf");
```

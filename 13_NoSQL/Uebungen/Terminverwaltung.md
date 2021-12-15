# Ein Buchungssystem

Für unseren Infotag konnten sich BesucherInnen für
den Infotag vorregistrieren. Es können Termine für verschiedene Abteilungen definiert
werden.

Ein Termin kann durchaus mehrfach vergeben werden. Vergleichen Sie die Situation mit
den Impfkojen im Austria Center. Wenn Sie 10 Kojen (= Stationen) haben, dann können auch 10 BesucherInnen
um 15:00 erscheinen. Die Anzahl der Stationen kann pro Abteilung individuell festgelegt
werden.

Parallel gibt es auch online Termine. Diese können unabhängig definiert werden (Zeit und
Anzahl der Stationen). Vergleichbar mit Impf- und Testkojen.

Wie auf der Buchungsmaske ersichtlich gibt es bis zu 2 BesucherInnen. Ein Hauptbesucher,
der Name, Telefonnummer und E-Mail Adresse hinterlässt. Der 2. Besucher ist optional und muss nur Name und Vorname angeben.

![](terminbuchung_besuchermaske.png)

Hinweise zur Umsetzung
- Die zentrale Klasse ist die Klasse *Termin*.
- Ein Termin kann mehrere Stationen haben.
- Ein Termin kann ein online oder offline Termin sein (Vererbung)
- Der online Termin speichert noch einen Meeting Link.
- Der offline Termin kann einen 2. Besucher (Begleiter) haben.
- Die BesucherInnen können in einer Liste gespeichert werden.
- Besucher und Begleiterproblem: Sie müssen immer eindeutig den Zusammenhang zwischen
  Besucher und Begleiter herstellen können.

### Aufgabe

Erstellen Sie ein neues MD File in Visual Studio Code mit dem Namen *Terminverwaltung.md*
Definieren Sie Ihr PlantUML Klassendiagramm in dieser Datei.

Danach können Sie im Musterprogramm zu "Find" im Ordner *03_MongoDb_Find*
diese Klassen in C# definieren. Dafür müssen Sie das Repository klonen, falls Sie das
nicht schon gemacht haben (`git clone https://github.com/schletz/Dbi3Sem.git`). Achten Sie auf die Konstruktoren und Datentypen.

## Eine mögliche Lösung

![](terminverwaltung_modell.svg)

```plantuml
hide empty methods

@startuml
class Besucher {
   *Vorname : String
   *Nachname : String
 
}

class Abteilung <<(D,#FF7700) Document>> {
   *Name : String <<Id>>
   *MaxBesucher : int
}

enum Terminart {
   Online
   Offline
}

class Termin <<(D,#FF7700) Document>> {
   *Id : Guid
   *Datum : DateTime
   *Von : DateTime
   *Abteilung : Abteilung
   *Terminart : Terminart
   Link: string
   *List<Anmeldung> : Anmeldungen
   ---
}
Termin *--> Abteilung
Termin *--> Terminart
Termin *--> Anmeldung

class Anmeldung {
   *Email : String
   *Telefon : String     
   *Anmelder : Besucher
   *Begleiter : List<Besucher>
}

Anmeldung *--> Besucher
@enduml
```

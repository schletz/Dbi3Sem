# Serialisierung von JSON Dateien

## Newtonsoft Json

Für das folgende Programm benötigen Sie das Paket
[Netwonsoft Json](https://www.nuget.org/packages/Newtonsoft.Json/). Das Paket können Sie wie
[in dieser Anleitung beschrieben](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio)
in Visual Studio installieren.

```c#
// *************************************************************************************************
// JSONSERIALIZER von Newtonsoft ohne Loop Detection
// Demonstriert die Methoden SerializeObject und DeserializeObject von Newtonsoft JSON.
//
// Zur Steuerung gibt es 2 wichtige Annotations:
//     [JsonIgnore]    Ignoriert das Property bei der Serialisierung. Dies ist bei den Rückreferenzen
//                     wichtig, da sonst eine self referencing Loop entsteht.
//     [JsonProperty("NameImJson")]    Wenn ein Property in der JSON Datei anders benannt werden soll,
//                                     dann kann dies hier angegeben werden.
//
// Weitere Infos sind unter https://www.newtonsoft.com/json/help/html/SerializeObject.htm zu finden.
// In der API Beschreibung können auch die Einzelheiten von benutzerdefinierten ContractResolvern
// nachgelesen werden: https://www.newtonsoft.com/json/help/html/N_Newtonsoft_Json_Serialization.htm
//
// Diese Art der Umwandlung funktioniert allerdings nur bei Datenmodellen, dessen Tabellen nur einen
// Femdschlüssel aufweisen und die 1:n Beziehungen sich somit als Baum darstellen lassen. 
// Wenn wir hier z. B. die Klasse Unterricht, die sich auf Lehrer und Schüler bezieht, einfügen, 
// schlägt das Konzept fehl. Denn wenn wir unter dem Schüler den Unterricht speichern, wird der 
// Lehrer, auf den im Unterricht verwiesen wird, neu angelegt.
//
// +--------+         +--------+
// | Schule | ----->> | Lehrer |
// |        |         +--------+
// |        |
// |        |         +--------+         +----------+
// |        | ----->> | Klasse | ----->> | Schueler |
// +--------+         +--------+         +----------+
// *************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonTest
{
    /// <summary>
    /// Modelklasse Schule mit Navigation zur Klasse.
    /// </summary>
    class Schule
    {
        public int Schulkennzahl { get; set; }
        public IList<Klasse> Klassen { get; set; } = new List<Klasse>();
        [JsonProperty("Lehrer")]              // Wir sollen im JSON das Property Lehrer dafür haben.
        public IList<Lehrer> Lehrers { get; set; } = new List<Lehrer>();
    }
    /// <summary>
    /// Modelklasse Klasse mit Rücknavigation zur Schule und Navigation zu den Schülern.
    /// </summary>
    class Klasse
    {
        public string Name { get; set; }
        public string Raum { get; set; }
        [JsonIgnore]                           // Wichtig, sonst gibt es eine self referencing loop.
        public Schule _Schule { get; set; }
        public IList<Schueler> Schueler { get; set; } = new List<Schueler>();
    }

    class Lehrer
    {
        public string Name { get; set; }
        [JsonIgnore]                           // Wichtig, sonst gibt es eine self referencing loop.
        public Schule _Schule { get; set; }
    }

    /// <summary>
    /// Modelklasse Schüler mit Rückreferenz zu der Klasse.
    /// </summary>
    class Schueler
    {
        public int Id { get; set; }
        [JsonProperty("Geburtsdatum")]                 // Damit wir auch andere Propertynamen haben.
        public DateTime Gebdat { get; set; }
        public string Vorname { get; set; }
        public string Zuname { get; set; }
        [JsonIgnore]                           // Wichtig, sonst gibt es eine self referencing loop.
        public Klasse _Klasse { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Wir generieren ein paar Testdaten.
            Schule schule = GenerateData();

            string json = JsonConvert.SerializeObject(schule, Formatting.Indented);
            Console.WriteLine("SERIALISIERUNG");
            Console.WriteLine(json);

            // DESERIALISIERUNG MIT JsonConvert.DeserializeObject<T>
            Schule schule2 = JsonConvert.DeserializeObject<Schule>(json);
            // Auch hier müssen wir die Rückreferenzen nachträglich setzen.
            foreach (Klasse k in schule2.Klassen)
            {
                k._Schule = schule2;
                foreach (Schueler s in k.Schueler) { s._Klasse = k; }
            }
            foreach (Lehrer l in schule2.Lehrers) { l._Schule = schule2; }

            if (json == JsonConvert.SerializeObject(schule2, Formatting.Indented))
            {
                Console.WriteLine("Die Serialisierungen von schule und schule2 sind ident. Es konnten also alle Informationen wiederhergestellt werden.");
            }
            Console.ReadLine();
        }

        public static Schule GenerateData()
        {
            Schule schule = new Schule
            {
                Schulkennzahl = 905417,
                Lehrers = new List<Lehrer>
                {
                    new Lehrer{Name = "Musterprof1"},
                    new Lehrer{Name = "Musterprof2"},
                    new Lehrer{Name = "Musterprof3"},
                },
                Klassen = new List<Klasse>
                {
                    new Klasse
                    {
                        Name = "3AHIF",
                        Raum = "C3.01",
                        Schueler = new List<Schueler>
                        {
                            new Schueler{Id = 11, Gebdat = new DateTime(2000,1,11), Vorname = "Mustervorname11", Zuname = "Musterzuname11"},
                            new Schueler{Id = 12, Gebdat = new DateTime(2000,1,12), Vorname = "Mustervorname12", Zuname = "Musterzuname12"},
                            new Schueler{Id = 13, Gebdat = new DateTime(2000,1,13), Vorname = "Mustervorname13", Zuname = "Musterzuname13"}
                        }
                    },
                    new Klasse
                    {
                        Name = "3BHIF",
                        Raum = "C3.02",
                        Schueler = new List<Schueler>
                        {
                            new Schueler{Id = 21, Gebdat = new DateTime(2000,1,21), Vorname = "Mustervorname21", Zuname = "Musterzuname21"},
                            new Schueler{Id = 22, Gebdat = new DateTime(2000,1,22), Vorname = "Mustervorname22", Zuname = "Musterzuname22"},
                            new Schueler{Id = 23, Gebdat = new DateTime(2000,1,23), Vorname = "Mustervorname23", Zuname = "Musterzuname23"}
                        }
                    }
                }
            };
            // Die Rückreferenzen müssen wir nachträglich setzen.
            foreach (Klasse k in schule.Klassen)
            {
                k._Schule = schule;
                foreach (Schueler s in k.Schueler) { s._Klasse = k; }
            }
            foreach (Lehrer l in schule.Lehrers) { l._Schule = schule; }
            return schule;
        }
    }
}
```

### Ausgabe

```javascript
{
  "Schulkennzahl": 905417,
  "Klassen": [
    {
      "Name": "3AHIF",
      "Raum": "C3.01",
      "Schueler": [
        {
          "Id": 11,
          "Geburtsdatum": "2000-01-11T00:00:00",
          "Vorname": "Mustervorname11",
          "Zuname": "Musterzuname11"
        },
        {
          "Id": 12,
          "Geburtsdatum": "2000-01-12T00:00:00",
          "Vorname": "Mustervorname12",
          "Zuname": "Musterzuname12"
        },
        {
          "Id": 13,
          "Geburtsdatum": "2000-01-13T00:00:00",
          "Vorname": "Mustervorname13",
          "Zuname": "Musterzuname13"
        }
      ]
    },
    {
      "Name": "3BHIF",
      "Raum": "C3.02",
      "Schueler": [
        {
          "Id": 21,
          "Geburtsdatum": "2000-01-21T00:00:00",
          "Vorname": "Mustervorname21",
          "Zuname": "Musterzuname21"
        },
        {
          "Id": 22,
          "Geburtsdatum": "2000-01-22T00:00:00",
          "Vorname": "Mustervorname22",
          "Zuname": "Musterzuname22"
        },
        {
          "Id": 23,
          "Geburtsdatum": "2000-01-23T00:00:00",
          "Vorname": "Mustervorname23",
          "Zuname": "Musterzuname23"
        }
      ]
    }
  ],
  "Lehrer": [
    {
      "Name": "Musterprof1"
    },
    {
      "Name": "Musterprof2"
    },
    {
      "Name": "Musterprof3"
    }
  ]
}
```

## Übung

In der Datei [Wetterwarnungen.json](Wetterwarnungen.json) finden Sie JSON Daten, wo Wetterwarnungen
für verschiedene Stationen gespeichert sind. Das Lesen mit Newtonsoft JSON ist auf https://www.newtonsoft.com/json/help/html/ReadJson.htm
beschrieben.

- Schreiben Sie C# Modelklassen, die den Inhalt dieser JSON Datei speichern können.
- Installieren Sie das Paket Newtonsoft JSON.
- Laden Sie mit Hilfe von Newtonsoft JSON diese Datei.
- Fügen Sie in der Modelklasse *Warnungen* Navigation Properties hinzu, sodass sie nicht nur die ID
  der Station und des Warntextes beinhalten, sondern auch auf den betreffenden Datensatz verweisen.
- Nach dem Laden der Datei setzen sie in der Main Methode die Navigation Properties, indem Sie
  - durch alle Warnungen mit einer Schleife durchgehen.
  - mit einer Schleife die ID der Stations in der Liste der Stationen suchen.
  - die Navigation auf diesen Datensatz setzen.
- Geben Sie zur Kontrolle das Dokument mit den gesetzten Properties als JSON auf der Konsole aus.
  Was fällt Ihnen auf? Können Sie die Ausgabe des gesamten Stationsdatensatzes verhindern?
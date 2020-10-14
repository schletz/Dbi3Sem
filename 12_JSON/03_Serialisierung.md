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

            // SERIALISIERUNG MIT JsonConvert.SerializeObject
            // {
            //   "Schulkennzahl": 905417,
            //   "Klassen": [
            //     {
            //       "Name": "3AHIF",
            //       "Raum": "C3.01",
            //       "Schueler": [
            //         {
            //           "Id": 11,
            //           "Geburtsdatum": "2000-01-11T00:00:00",
            //           "Vorname": "Mustervorname11",
            //           "Zuname": "Musterzuname11"
            //         },
            //         {
            //           "Id": 12,
            //           "Geburtsdatum": "2000-01-12T00:00:00",
            //           "Vorname": "Mustervorname12",
            //           "Zuname": "Musterzuname12"
            //         },
            //         {
            //           "Id": 13,
            //           "Geburtsdatum": "2000-01-13T00:00:00",
            //           "Vorname": "Mustervorname13",
            //           "Zuname": "Musterzuname13"
            //         }
            //       ]
            //     },
            //     {
            //       "Name": "3BHIF",
            //       "Raum": "C3.02",
            //       "Schueler": [
            //         {
            //           "Id": 21,
            //           "Geburtsdatum": "2000-01-21T00:00:00",
            //           "Vorname": "Mustervorname21",
            //           "Zuname": "Musterzuname21"
            //         },
            //         {
            //           "Id": 22,
            //           "Geburtsdatum": "2000-01-22T00:00:00",
            //           "Vorname": "Mustervorname22",
            //           "Zuname": "Musterzuname22"
            //         },
            //         {
            //           "Id": 23,
            //           "Geburtsdatum": "2000-01-23T00:00:00",
            //           "Vorname": "Mustervorname23",
            //           "Zuname": "Musterzuname23"
            //         }
            //       ]
            //     }
            //   ],
            //   "Lehrer": [
            //     {
            //       "Name": "Musterprof1"
            //     },
            //     {
            //       "Name": "Musterprof2"
            //     },
            //     {
            //       "Name": "Musterprof3"
            //     }
            //   ]
            // }
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

## Newtonsoft JSON Serializer mit PreserveReferencesHandling

```c#
// *************************************************************************************************
// JSONSERIALIZER von Newtonsoft mit Loop Detection
// Demonstriert die Methoden SerializeObject und DeserializeObject von Newtonsoft JSON.
//
// Als weiterer Parameter kann bei JsonConvert.SerializeObject und JsonConvert.DeserializeObject<T>
// new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
// angegeben werden. Hier bekommt jede Instanz ein Property $id. Auf diese ID wird dann mit dem
// Property $ref verwiesen. Somit können auch ganze Objektmodelle wie das Folgende einfach
// serialisiert werden:
//
// +--------+         +--------+         +------------+
// | Schule | ----->> | Lehrer | ----->> | Unterricht |
// |        |         |        |   +-->> |            |
// |        |         +--------+   |     +------------+
// |        |                      |
// |        |         +--------+   |     +----------+
// |        | ----->> | Klasse | --+     | Schueler |
// |        |         |        | ----->> |          |
// +--------+         +--------+         +----------+
//
// Weitere Infos auf https://www.newtonsoft.com/json/help/html/PreserveObjectReferences.htm
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
        public Schule _Schule { get; set; }
        public IList<Schueler> Schueler { get; set; } = new List<Schueler>();
        public List<Unterricht> Unterrichte { get; set; } = new List<Unterricht>();
    }

    /// <summary>
    /// Modelklasse Lehrer mit Rücknavigation zur Schule.
    /// </summary>
    class Lehrer
    {
        public string Name { get; set; }
        public Schule _Schule { get; set; }
        public List<Unterricht> Unterrichte { get; set; } = new List<Unterricht>();
    }

    /// <summary>
    /// Modelklasse Unterricht mit den Rücknavigationen zum Lehrer und zur Klasse.
    /// </summary>
    class Unterricht
    {
        public string Fach { get; set; }
        public Lehrer _Lehrer { get; set; }
        public Klasse _Klasse { get; set; }
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
        public Klasse _Klasse { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Wir generieren ein paar Testdaten.
            Schule schule = GenerateData();

            // SERIALISIERUNG MIT JsonConvert.SerializeObject
            // {
            //   "$id": "1",
            //   "Schulkennzahl": 905417,
            //   "Klassen": [
            //     {
            //       "$id": "2",
            //       "Name": "3AHIF",
            //       "Raum": "C3.01",
            //       "_Schule": {
            //         "$ref": "1"
            //       },
            //       "Schueler": [
            //         {
            //           "$id": "3",
            //           "Id": 11,
            //           "Geburtsdatum": "2000-01-11T00:00:00",
            //           "Vorname": "Mustervorname11",
            //           "Zuname": "Musterzuname11",
            //           "_Klasse": {
            //             "$ref": "2"
            //           }
            //         },
            //         {
            //           "$id": "4",
            //           "Id": 12,
            //           "Geburtsdatum": "2000-01-12T00:00:00",
            //           "Vorname": "Mustervorname12",
            //           "Zuname": "Musterzuname12",
            //           "_Klasse": {
            //             "$ref": "2"
            //           }
            //         },
            //         {
            //           "$id": "5",
            //           "Id": 13,
            //           "Geburtsdatum": "2000-01-13T00:00:00",
            //           "Vorname": "Mustervorname13",
            //           "Zuname": "Musterzuname13",
            //           "_Klasse": {
            //             "$ref": "2"
            //           }
            //         }
            //       ],
            //       "Unterrichte": [
            //         {
            //           "$id": "6",
            //           "Fach": "AM",
            //           "_Lehrer": {
            //             "$id": "7",
            //             "Name": "Musterprof1",
            //             "_Schule": {
            //               "$ref": "1"
            //             },
            //             "Unterrichte": [
            //               {
            //                 "$ref": "6"
            //               },
            //               {
            //                 "$id": "8",
            //                 "Fach": "D",
            //                 "_Lehrer": {
            //                   "$ref": "7"
            //                 },
            //                 "_Klasse": {
            //                   "$id": "9",
            //                   "Name": "3BHIF",
            //                   "Raum": "C3.02",
            //                   "_Schule": {
            //                     "$ref": "1"
            //                   },
            //                   "Schueler": [
            //                     {
            //                       "$id": "10",
            //                       "Id": 21,
            //                       "Geburtsdatum": "2000-01-21T00:00:00",
            //                       "Vorname": "Mustervorname21",
            //                       "Zuname": "Musterzuname21",
            //                       "_Klasse": {
            //                         "$ref": "9"
            //                       }
            //                     },
            //                     {
            //                       "$id": "11",
            //                       "Id": 22,
            //                       "Geburtsdatum": "2000-01-22T00:00:00",
            //                       "Vorname": "Mustervorname22",
            //                       "Zuname": "Musterzuname22",
            //                       "_Klasse": {
            //                         "$ref": "9"
            //                       }
            //                     },
            //                     {
            //                       "$id": "12",
            //                       "Id": 23,
            //                       "Geburtsdatum": "2000-01-23T00:00:00",
            //                       "Vorname": "Mustervorname23",
            //                       "Zuname": "Musterzuname23",
            //                       "_Klasse": {
            //                         "$ref": "9"
            //                       }
            //                     }
            //                   ],
            //                   "Unterrichte": [
            //                     {
            //                       "$ref": "8"
            //                     },
            //                     {
            //                       "$id": "13",
            //                       "Fach": "POS",
            //                       "_Lehrer": {
            //                         "$id": "14",
            //                         "Name": "Musterprof2",
            //                         "_Schule": {
            //                           "$ref": "1"
            //                         },
            //                         "Unterrichte": [
            //                           {
            //                             "$ref": "13"
            //                           }
            //                         ]
            //                       },
            //                       "_Klasse": {
            //                         "$ref": "9"
            //                       }
            //                     }
            //                   ]
            //                 }
            //               }
            //             ]
            //           },
            //           "_Klasse": {
            //             "$ref": "2"
            //           }
            //         }
            //       ]
            //     },
            //     {
            //       "$ref": "9"
            //     }
            //   ],
            //   "Lehrer": [
            //     {
            //       "$ref": "7"
            //     },
            //     {
            //       "$ref": "14"
            //     },
            //     {
            //       "$id": "15",
            //       "Name": "Musterprof3",
            //       "_Schule": {
            //         "$ref": "1"
            //       },
            //       "Unterrichte": []
            //     }
            //   ]
            // }
            // 
            string json = JsonConvert.SerializeObject(schule,
                Formatting.Indented,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            Console.WriteLine("SERIALISIERUNG");
            Console.WriteLine(json);

            // DESERIALISIERUNG MIT JsonConvert.DeserializeObject<T>
            Schule schule2 = JsonConvert.DeserializeObject<Schule>(json,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            // Hier brauchen wir die Rückreferenzen NICHT händisch nachziehen, denn sie werden vom
            // Serializer richtig wiederhergestellt.
            if (json == JsonConvert.SerializeObject(schule2,
                                                    Formatting.Indented,
                                                    new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }))
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

            // Jetzt kommen ein paar Unterrichte.
            Unterricht u1 = new Unterricht { _Lehrer = schule.Lehrers[0], _Klasse = schule.Klassen[0], Fach = "AM" };
            schule.Lehrers[0].Unterrichte.Add(u1);
            schule.Klassen[0].Unterrichte.Add(u1);
            Unterricht u2 = new Unterricht { _Lehrer = schule.Lehrers[0], _Klasse = schule.Klassen[1], Fach = "D" };
            schule.Lehrers[0].Unterrichte.Add(u2);
            schule.Klassen[1].Unterrichte.Add(u2);
            Unterricht u3 = new Unterricht { _Lehrer = schule.Lehrers[1], _Klasse = schule.Klassen[1], Fach = "POS" };
            schule.Lehrers[1].Unterrichte.Add(u3);
            schule.Klassen[1].Unterrichte.Add(u3);

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

## Übung

In der letzten Übung 2 zu [Modelklassen erstellen](02_Modelklassen.md) erstellten Sie eine
Datenstruktur, die den Stundenplan der Schule abbildet. Erstellen Sie nun ein .NET Programm,
welches diese Datenstruktur mit einigen Musterdaten befüllt und das Modell in eine JSON Datei
mit Hilfe Newtonsoft Json schreibt. 

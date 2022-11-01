using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace SalesDbGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

            var salesDb = SalesDatabase.FromConnectionString("mongodb://root:1234@localhost:27017", logging: true);
            try
            {
                salesDb.Seed();
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("Die Datenbank ist nicht erreichbar. Läuft der Container?");
                return 1;
            }
            catch (MongoAuthenticationException)
            {
                Console.Error.WriteLine("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
                return 2;
            }

            var options = new System.Text.Json.JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),  // Umlaute als Umlaut ausgeben (nicht als \uxxxx)
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

            // HINWEIS FÜR DEN ZUGRIFF AUF COLLECTIONS
            // In der Klasse SalesDatabase ist ein Property für jede Collection definiert. So kann mit
            // examsDb.Products auf products zugegriffen werden. Damit erspart man sich das Schreiben
            // von salesDb.Db.GetCollection<Product>("products").
            //
            // salesDb.Products  entspricht salesDb.Db.GetCollection<Product>("products")
            // salesDb.Customer  entspricht salesDb.Db.GetCollection<Customer>("customers")
            // salesDb.Order  entspricht salesDb.Db.GetCollection<Order>("orders")

            // Hinweis zum Logging: Soll die generierte Shell Anweisung ausgegeben werden, setze
            // logging in Zeile 17 auf true.

            // *****************************************************************************************
            // FILTERABFRAGEN
            // *****************************************************************************************
            // Muster: Anzahl der negativen Prüfungen pro Fach
            {
                PrintHeader("Muster: Produkt mit der EAN 317174.");
                var result = salesDb.Products.AsQueryable().Where(p => p.Ean == "317174");
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.1) Produkte der Kategorie Electronics.");
                var result = Enumerable.Empty<Product>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.2) Produkte, die unter 400 Euro kosten.");
                var result = Enumerable.Empty<Product>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            // 
            {
                PrintHeader("(1.3) Produkte, die ab 1.1.2022 nicht mehr verfügbar sind.");
                var result = Enumerable.Empty<Product>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            // 
            {
                PrintHeader("(1.4) Produkte, die keinen Wert in AvailableTo haben.");
                var result = Enumerable.Empty<Product>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.5) Produkte, wo der Lagerstand unter dem minimalen Lagerstand liegt.");
                var result = Enumerable.Empty<Product>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            // 
            {
                PrintHeader("(1.6) Kunden, die eine Adresse im Burgenland in shippingAddresses haben.");
                var result = Enumerable.Empty<Customer>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }


            // *****************************************************************************************
            {
                PrintHeader("(1.7) Kunden, mehr als 2 Adressen in shippingAddresses haben.");
                var result = Enumerable.Empty<Customer>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.8) Orders des Produktes 566572.");
                var result = Enumerable.Empty<Order>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }


            // *****************************************************************************************
            {
                PrintHeader("(1.9) Orders, in denen ein Produkt vorkommt, dessen ItemPrice mehr als 990 Euro gekostet hat.");
                var result = Enumerable.Empty<Order>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.10) Orders ohne ein Produkt der Kategorie Sportswear.");
                var result = Enumerable.Empty<Order>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            {
                PrintHeader("(1.11) Orders, die nur Produkte der Kategorie Sportswear haben.");
                var result = Enumerable.Empty<Order>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.Id.ToString().Substring(16, 8))));
            }

            // *****************************************************************************************
            // AGGREGATE
            // *****************************************************************************************
            {
                PrintHeader("(2.1) Anzahl der Produkte pro Kategorie.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.2) Anzahl der Produkte pro Kategorie, sortiert nach der Kategorie.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.3) Anzahl der Produkte pro Kategorie, absteigend sortiert nach der Anzahl.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            // Hinweis: Gib die Id mit .ToString().Substring(16,8) aus.
            {
                PrintHeader("(2.4) Kunden und die Anzahl der Adressen in shippingAddresses.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            // "addresses": {"$setUnion": [ "$shippingAddresses.state"]},
            {
                PrintHeader("(2.5) Bundesländer der Kunden in shippingAddresses.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            // $unwind
            {
                PrintHeader("(2.6) Bundesländer und Anzahl der Einträge, die sie in shippingAddresses haben.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            // 2x $group
            {
                PrintHeader("(2.7) Bundesländer und Anzahl der Kunden, wo dieses Bundesland in shippingAddresses vorkommt.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.8) Umsatz pro Kunde.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.9) Durchschnittlicher Verkaufspreis eines Produktes.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.10) Umsatz pro Jahr und Monat, sortiert nach Jahr und Monat");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }


            // *****************************************************************************************
            {
                PrintHeader("(2.11) ItemPrice < 92% von Recommended Price.");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            // *****************************************************************************************
            {
                PrintHeader("(2.12) Umsatz pro Bundesland");
                var result = Enumerable.Empty<object>();  // TODO: Schreibe hier deine Abfrage.
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, options));
            }

            return 0;
        }
        static void PrintHeader(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Environment.NewLine + text);
            Console.ForegroundColor = color;
        }
    }
}

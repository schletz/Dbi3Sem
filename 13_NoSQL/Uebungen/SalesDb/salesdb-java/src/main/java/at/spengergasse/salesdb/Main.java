package at.spengergasse.salesdb;

import java.util.ArrayList;
import java.util.stream.Collectors;

import com.mongodb.MongoSecurityException;
import com.mongodb.MongoTimeoutException;
import com.mongodb.client.model.Filters;

import at.spengergasse.salesdb.infrastructure.SalesDatabase;
import at.spengergasse.salesdb.model.Customer;
import at.spengergasse.salesdb.model.Order;
import at.spengergasse.salesdb.model.Product;

public class Main {
    public static void main(String[] args) {
        var salesDatabase = SalesDatabase.fromConnectionString("mongodb://root:1234@localhost:27017");
        try {
            salesDatabase.Seed();
        } catch (MongoTimeoutException e) {
            System.err.println("Die Datenbank ist nicht erreichbar. Läuft der Container?");
            System.exit(1);
            return;
        } catch (MongoSecurityException e) {
            System.err.println("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
            System.exit(2);
            return;
        }

        catch (Exception e) {
            System.err.println(e.getMessage());
            System.exit(3);
            return;
        }

        {
            var db = salesDatabase.getDb();

            var customers = db.getCollection("customers", Customer.class).find().into(new ArrayList<>());
            var orders = db.getCollection("orders", Order.class).find().into(new ArrayList<>());
            var products = db.getCollection("products", Product.class).find().into(new ArrayList<>());

            System.out.println(String.format("%d Dokumente in customers gelesen", customers.size()));
            System.out.println(String.format("%d Dokumente in orders gelesen", orders.size()));
            System.out.println(String.format("%d Dokumente in products gelesen", products.size()));
        }
        // Für den leichteren Zugriff auf die Collections stellt die Klasse
        // SalesDatabase
        // folgende Methoden bereit:
        // MongoCollection<Customer> getCustomers()
        // MongoCollection<Order> getOrders()
        // MongoCollection<Product> getProducts()
        // *****************************************************************************************
        // FILTERABFRAGEN
        // *****************************************************************************************
        {
            System.out.println("Muster: Produkt mit der EAN 317174.");
            var result = salesDatabase.getProducts()
                    .find(Filters.eq("ean", "317174"))
                    .into(new ArrayList<>());
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }
        // *****************************************************************************************
        {
            System.out.println("(1.1) Produkte der Kategorie Electronics.");
            var result = new ArrayList<Product>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.2) Produkte, die unter 400 Euro kosten.");
            var result = new ArrayList<Product>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        //
        {
            System.out.println("(1.3) Produkte, die ab 1.1.2022 nicht mehr verfügbar sind.");
            var result = new ArrayList<Product>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        //
        {
            System.out.println("(1.4) Produkte, die keinen Wert in AvailableTo haben.");
            var result = new ArrayList<Product>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.5) Produkte, wo der Lagerstand unter dem minimalen Lagerstand liegt.");
            var result = new ArrayList<Product>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        //
        {
            System.out.println("(1.6) Kunden, die eine Adresse im Burgenland in shippingAddresses haben.");
            var result = new ArrayList<Customer>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.7) Kunden, mehr als 2 Adressen in shippingAddresses haben.");
            var result = new ArrayList<Customer>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.8) Orders des Produktes 566572.");
            var result = new ArrayList<Order>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println(
                    "(1.9) Orders, in denen ein Produkt vorkommt, dessen ItemPrice mehr als 990 Euro gekostet hat.");
            var result = new ArrayList<Order>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.10) Orders ohne ein Produkt der Kategorie Sportswear.");
            var result = new ArrayList<Order>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }

        // *****************************************************************************************
        {
            System.out.println("(1.11) Orders, die nur Produkte der Kategorie Sportswear haben.");
            var result = new ArrayList<Order>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(
                    result.stream().map(r -> r.id().toHexString().substring(16, 24)).collect(Collectors.joining(", ")));
        }
        // *****************************************************************************************
        // AGGREGATE
        // *****************************************************************************************
        {
            System.out.println("(2.1) Anzahl der Produkte pro Kategorie.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.2) Anzahl der Produkte pro Kategorie, sortiert nach der Kategorie.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.3) Anzahl der Produkte pro Kategorie, absteigend sortiert nach der Anzahl.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        // Hinweis: Gib die Id mit .ToString().Substring(16,8) aus.
        {
            System.out.println("(2.4) Kunden und die Anzahl der Adressen in shippingAddresses.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        // "addresses": {"$setUnion": [ "$shippingAddresses.state"]},
        {
            System.out.println("(2.5) Bundesländer der Kunden in shippingAddresses.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.6) Bundesländer und Anzahl der Einträge, die sie in shippingAddresses haben.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println(
                    "(2.7) Bundesländer und Anzahl der Kunden, wo dieses Bundesland in shippingAddresses vorkommt.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.8) Umsatz pro Kunde.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.9) Durchschnittlicher Verkaufspreis eines Produktes.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.10) Umsatz pro Jahr und Monat, sortiert nach Jahr und Monat");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.11) ItemPrice < 92% von Recommended Price.");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }

        // *****************************************************************************************
        {
            System.out.println("(2.12) Umsatz pro Bundesland");
            var result = new ArrayList<Object>(); // TODO: Schreibe hier deine Abfrage.
            System.out.println(result);
        }
    }
}
package at.spengergasse.salesdb.infrastructure;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.nio.charset.Charset;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

import org.bson.Document;
import org.bson.codecs.configuration.CodecRegistries;
import org.bson.codecs.configuration.CodecRegistry;
import org.bson.codecs.pojo.PojoCodecProvider;
import org.slf4j.LoggerFactory;

import com.mongodb.ConnectionString;
import com.mongodb.MongoClientSettings;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import com.mongodb.client.model.InsertOneModel;

import at.spengergasse.salesdb.converters.LocalDateCodec;
import at.spengergasse.salesdb.converters.LocalTimeCodec;
import at.spengergasse.salesdb.converters.ZonedDateTimeCodec;
import at.spengergasse.salesdb.model.Customer;
import at.spengergasse.salesdb.model.Order;
import at.spengergasse.salesdb.model.Product;
import ch.qos.logback.classic.Level;
import ch.qos.logback.classic.Logger;

public class SalesDatabase {
    private final MongoClient client;
    private final MongoDatabase db;

    public static SalesDatabase fromConnectionString(String connectionString) {
        return SalesDatabase.fromConnectionString(connectionString, false);
    }

    /**
     * Initialisiert den MongoClient mit den notwendigen Codecs und konfiguriert das
     * MongoDatabase
     * Objekt für die Verbindung.
     * 
     * @param connectionString Verbindungsstring mit dem Aufbau
     *                         mongodb://user:pass@localhost:27017
     * @param enableLogging    true, wenn der Logger alle Infos auf der Konsole
     *                         ausgeben soll.
     * @return
     */
    public static SalesDatabase fromConnectionString(String connectionString, Boolean enableLogging) {
        ((Logger) LoggerFactory.getLogger(Logger.ROOT_LOGGER_NAME))
                .setLevel(enableLogging ? Level.DEBUG : Level.ERROR);

        var client = MongoClients.create(
                MongoClientSettings.builder()
                        .applyToClusterSettings(builder -> builder.serverSelectionTimeout(5, TimeUnit.SECONDS))
                        .applyConnectionString(new ConnectionString(connectionString))
                        .build());
        CodecRegistry pojoCodecRegistry = CodecRegistries.fromRegistries(
                CodecRegistries.fromCodecs(
                        new LocalDateCodec(), new LocalTimeCodec(), new ZonedDateTimeCodec()),
                MongoClientSettings.getDefaultCodecRegistry(),
                CodecRegistries.fromProviders(PojoCodecProvider.builder().automatic(true).build()));

        var db = client.getDatabase("salesDb").withCodecRegistry(pojoCodecRegistry);
        return new SalesDatabase(client, db);
    }

    private SalesDatabase(MongoClient client, MongoDatabase db) {
        this.client = client;
        this.db = db;
    }

    public MongoDatabase getDb() {
        return db;
    }

    public MongoClient getClient() {
        return client;
    }

    public MongoCollection<Customer> getCustomers() {
        return db.getCollection("customers", Customer.class);
    }

    public MongoCollection<Order> getOrders() {
        return db.getCollection("orders", Order.class);
    }

    public MongoCollection<Product> getProducts() {
        return db.getCollection("products", Product.class);
    }

    /**
     * Liest die JSON Dumps aus dem resource/dumps Ordner und fügt sie in die
     * Datenbank ein. Löscht vorher die Collection.
     * 
     * @throws FileNotFoundException
     * @throws IOException
     */
    public void Seed() throws FileNotFoundException, IOException {
        var collections = new String[] { "customers", "orders", "products" };

        for (var collection : collections) {
            var filename = getClass().getClassLoader().getResource("dump/" + collection + ".json").getFile();
            if (filename.isEmpty())
                throw new FileNotFoundException(
                        String.format("File %s not found. Check your resources/dump directory.", filename));
            try (var reader = new BufferedReader(new FileReader(filename, Charset.forName("UTF-8")))) {
                db.getCollection(collection).drop();
                db.getCollection(collection).bulkWrite(reader
                        .lines()
                        .map(line -> new InsertOneModel<Document>(Document.parse(line)))
                        .collect(Collectors.toList()));
            }
        }
    }
}

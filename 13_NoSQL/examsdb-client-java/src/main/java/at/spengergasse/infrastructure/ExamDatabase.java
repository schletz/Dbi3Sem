package at.spengergasse.infrastructure;

import at.spengergasse.converters.*;
import at.spengergasse.model.*;
import ch.qos.logback.classic.Level;
import ch.qos.logback.classic.Logger;
import lombok.Getter;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

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
import com.mongodb.client.model.BulkWriteOptions;
import com.mongodb.client.model.InsertOneModel;

@Getter
public class ExamDatabase {
    private final MongoClient client;
    private final MongoDatabase db;

    public static ExamDatabase fromConnectionString(String connectionString) {
        return ExamDatabase.fromConnectionString(connectionString, false);
    }

    public static ExamDatabase fromConnectionString(String connectionString, Boolean enableLogging) {
        ((Logger) LoggerFactory.getLogger(Logger.ROOT_LOGGER_NAME))
                .setLevel(enableLogging ? Level.INFO : Level.ERROR);

        var client = MongoClients.create(
                MongoClientSettings.builder()
                        .applyToClusterSettings(builder -> builder.serverSelectionTimeout(5, TimeUnit.SECONDS))
                        .applyConnectionString(new ConnectionString(connectionString))
                        .build());
        CodecRegistry pojoCodecRegistry = CodecRegistries.fromRegistries(
                MongoClientSettings.getDefaultCodecRegistry(),
                CodecRegistries.fromCodecs(new DateOnlyCodec(), new GenderCodec(), new TermTypeCodec(),
                        new TimeOnlyCodec()),
                CodecRegistries.fromProviders(PojoCodecProvider.builder().automatic(true).build()));

        var db = client.getDatabase("examsDb").withCodecRegistry(pojoCodecRegistry);
        return new ExamDatabase(client, db);
    }

    private ExamDatabase(MongoClient client, MongoDatabase db) {
        this.client = client;
        this.db = db;
    }

    public MongoCollection<SchoolClass> getClasses() {
        return db.getCollection("classes", SchoolClass.class);
    }

    public MongoCollection<Exam> getExams() {
        return db.getCollection("exams", Exam.class);
    }

    public MongoCollection<Room> getRooms() {
        return db.getCollection("rooms", Room.class);
    }

    public MongoCollection<Student> getStudents() {
        return db.getCollection("students", Student.class);
    }

    public MongoCollection<Subject> getSubjects() {
        return db.getCollection("subjects", Subject.class);
    }

    public MongoCollection<Teacher> getTeachers() {
        return db.getCollection("teachers", Teacher.class);
    }

    public MongoCollection<Term> getTerms() {
        return db.getCollection("terms", Term.class);
    }

    /**
     * Liest die JSON Dumps aus dem resource/dumps Ordner und fügt sie in die
     * Datenbank ein.
     * Löscht vorher die Collection.
     * 
     * @throws FileNotFoundException
     * @throws IOException
     */
    public void Seed() throws FileNotFoundException, IOException {
        var collections = new String[] { "terms", "subjects", "rooms", "classes", "students", "teachers", "exams" };

        for (var collection : collections) {
            List<InsertOneModel<Document>> docs = new ArrayList<>();

            var filename = "dump/" + collection + ".json";
            try (var inputStream = getClass().getClassLoader().getResourceAsStream(filename)) {
                if (inputStream == null) {
                    throw new FileNotFoundException();
                }
                try (var reader = new BufferedReader(new InputStreamReader(inputStream))) {
                    while (true) {
                        var line = reader.readLine();
                        if (line == null) {
                            break;
                        }
                        docs.add(new InsertOneModel<Document>(Document.parse(line)));
                    }
                    db.getCollection(collection).drop();
                    db.getCollection(collection).bulkWrite(docs, new BulkWriteOptions().ordered(false));
                }
            }
        }
    }
}

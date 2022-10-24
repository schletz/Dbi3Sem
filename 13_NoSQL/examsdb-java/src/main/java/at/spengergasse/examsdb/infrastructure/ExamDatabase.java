package at.spengergasse.examsdb.infrastructure;

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
import com.mongodb.client.model.Indexes;
import com.mongodb.client.model.InsertOneModel;

import at.spengergasse.examsdb.converters.GenderCodec;
import at.spengergasse.examsdb.converters.LocalDateCodec;
import at.spengergasse.examsdb.converters.LocalTimeCodec;
import at.spengergasse.examsdb.converters.TermTypeCodec;
import at.spengergasse.examsdb.converters.ZonedDateTimeCodec;
import at.spengergasse.examsdb.model.Exam;
import at.spengergasse.examsdb.model.Room;
import at.spengergasse.examsdb.model.SchoolClass;
import at.spengergasse.examsdb.model.Student;
import at.spengergasse.examsdb.model.Subject;
import at.spengergasse.examsdb.model.Teacher;
import at.spengergasse.examsdb.model.Term;
import ch.qos.logback.classic.Level;
import ch.qos.logback.classic.Logger;
import lombok.Getter;

@Getter
public class ExamDatabase {
    private final MongoClient client;
    private final MongoDatabase db;

    public static ExamDatabase fromConnectionString(String connectionString) {
        return ExamDatabase.fromConnectionString(connectionString, false);
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
    public static ExamDatabase fromConnectionString(String connectionString, Boolean enableLogging) {
        ((Logger) LoggerFactory.getLogger(Logger.ROOT_LOGGER_NAME))
                .setLevel(enableLogging ? Level.INFO : Level.ERROR);

        var client = MongoClients.create(
                MongoClientSettings.builder()
                        .applyToClusterSettings(builder -> builder.serverSelectionTimeout(5, TimeUnit.SECONDS))
                        .applyConnectionString(new ConnectionString(connectionString))
                        .build());
        CodecRegistry pojoCodecRegistry = CodecRegistries.fromRegistries(
                CodecRegistries.fromCodecs(
                        new GenderCodec(), new TermTypeCodec(),
                        new LocalDateCodec(), new LocalTimeCodec(), new ZonedDateTimeCodec()),
                MongoClientSettings.getDefaultCodecRegistry(),
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
     * Datenbank ein. Löscht vorher die Collection.
     * 
     * @throws FileNotFoundException
     * @throws IOException
     */
    public void Seed() throws FileNotFoundException, IOException {
        var collections = new String[] { "terms", "subjects", "rooms", "classes", "students", "teachers", "exams" };

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
        getClasses().createIndex(Indexes.ascending("term.year"));
        getExams().createIndex(Indexes.ascending("currentClass._id"));
        getExams().createIndex(Indexes.ascending("student.nr"));
        getExams().createIndex(Indexes.ascending("teacher.shortname"));
        getStudents().createIndex(Indexes.ascending("currentClass.shortname"));
    }
}

package at.spengergasse.examsdb;

import java.time.format.DateTimeFormatter;
import java.util.ArrayList;

import com.mongodb.MongoSecurityException;
import com.mongodb.MongoTimeoutException;
import com.mongodb.client.model.Filters;

import at.spengergasse.examsdb.infrastructure.ExamDatabase;
import at.spengergasse.examsdb.model.Exam;
import at.spengergasse.examsdb.model.Room;
import at.spengergasse.examsdb.model.SchoolClass;
import at.spengergasse.examsdb.model.Student;
import at.spengergasse.examsdb.model.Subject;
import at.spengergasse.examsdb.model.Teacher;
import at.spengergasse.examsdb.model.Term;

public class Main {
    public static void main(String[] args) {
        var examDatabase = ExamDatabase.fromConnectionString("mongodb://root:1234@localhost:27017");
        try {
            examDatabase.Seed();
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
            var db = examDatabase.getDb();

            var classes = db.getCollection("classes", SchoolClass.class).find().into(new ArrayList<>());
            var exams = db.getCollection("exams", Exam.class).find().into(new ArrayList<>());
            var rooms = db.getCollection("rooms", Room.class).find().into(new ArrayList<>());
            var students = db.getCollection("students", Student.class).find().into(new ArrayList<>());
            var subjects = db.getCollection("subjects", Subject.class).find().into(new ArrayList<>());
            var teachers = db.getCollection("teachers", Teacher.class).find().into(new ArrayList<>());
            var terms = db.getCollection("terms", Term.class).find().into(new ArrayList<>());

            System.out.println(String.format("%d Dokumente in classes gelesen", classes.size()));
            System.out.println(String.format("%d Dokumente in exams gelesen", exams.size()));
            System.out.println(String.format("%d Dokumente in rooms gelesen", rooms.size()));
            System.out.println(String.format("%d Dokumente in students gelesen", students.size()));
            System.out.println(String.format("%d Dokumente in subjects gelesen", subjects.size()));
            System.out.println(String.format("%d Dokumente in teachers gelesen", teachers.size()));
            System.out.println(String.format("%d Dokumente in terms gelesen", terms.size()));
        }
        // Für den leichteren Zugriff auf die Collections stellt die Klasse ExamDatabase
        // folgende Methoden bereit:
        // MongoCollection<SchoolClass> getClasses()
        // MongoCollection<Exam> getExams()
        // MongoCollection<Room> getRooms()
        // MongoCollection<Student> getStudents()
        // MongoCollection<Subject> getSubjects()
        // MongoCollection<Teacher> getTeachers()
        // MongoCollection<Term> getTerms()
        {
            System.out.println("Alle Klassen der AIF im Schuljahr 2022");
            examDatabase.getClasses()
                    .find(Filters.and(
                            Filters.eq("term.year", 2022),
                            Filters.eq("department", "AIF")))
                    .forEach(doc -> System.out.println(String.format("%s (KV: %s %s). Beginn: %s, Ende: %s.",
                            doc.id(), doc.classTeacher().firstname(), doc.classTeacher().lastname(),
                            doc.term().start().format(DateTimeFormatter.ISO_DATE),
                            doc.term().end().format(DateTimeFormatter.ISO_DATE))));
        }
    }
}
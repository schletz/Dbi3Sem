package at.spengergasse;
import at.spengergasse.infrastructure.ExamDatabase;
import at.spengergasse.model.*;

import java.time.format.DateTimeFormatter;
import java.util.ArrayList;

public class Main {
    public static void main(String[] args) {
        var examDatabase = ExamDatabase.fromConnectionString("mongodb://root:1234@localhost:27017");
        var db =examDatabase.getDb();

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
}
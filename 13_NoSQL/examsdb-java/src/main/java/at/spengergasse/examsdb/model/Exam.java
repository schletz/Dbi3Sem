package at.spengergasse.examsdb.model;

import java.time.ZonedDateTime;

import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.types.ObjectId;

public record Exam(@BsonId ObjectId id, StudentName student, TeacherName teacher,
        SchoolClass currentClass, SchoolClass examClass, Subject subject, ZonedDateTime dateTime,
        int pointsMax, int points, int grade) {
    public Exam {
    }

    public Exam(StudentName student, TeacherName teacher,
            SchoolClass currentClass, SchoolClass examClass, Subject subject, ZonedDateTime dateTime,
            int pointsMax, int points, int grade) {
        this(new ObjectId(), student, teacher, currentClass, examClass, subject, dateTime, pointsMax, points, grade);
    }
}
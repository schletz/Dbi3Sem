package at.spengergasse.examsdb.model;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;

public record Student(StudentName name, Gender gender, Address address, LocalDate dateOfBirth, SchoolClass currentClass,
        List<SchoolClass> classHistory) {
    public Student {
    }

    public Student(StudentName name, Gender gender, Address address, LocalDate dateOfBirth, SchoolClass currentClass) {
        this(name, gender, address, dateOfBirth, currentClass, new ArrayList<SchoolClass>());
    }

    @BsonId
    public int id() {
        return name.nr();
    }
}
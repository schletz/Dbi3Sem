package at.spengergasse.examsdb.model;

import java.math.BigDecimal;
import java.time.LocalTime;
import java.util.ArrayList;
import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;

public record Teacher(TeacherName name, Gender gender, Integer hoursPerWeek, LocalTime lessonsFrom, BigDecimal salary,
        List<String> homeOfficeDays, List<Subject> canTeachSubjects) {
    public Teacher(TeacherName name, Gender gender, Integer hoursPerWeek, LocalTime lessonsFrom, BigDecimal salary) {
        this(name, gender, hoursPerWeek, lessonsFrom, salary, new ArrayList<String>(), new ArrayList<Subject>());
    }

    @BsonId
    public String id() {
        return name.shortname();
    }
}
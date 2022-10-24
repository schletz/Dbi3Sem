package at.spengergasse.examsdb.model;

import java.time.ZonedDateTime;

import org.bson.types.ObjectId;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Exam {
    private StudentName student;
    private TeacherName teacher;
    private SchoolClass currentClass;
    private SchoolClass examClass;
    private Subject subject;
    private ZonedDateTime dateTime;
    private int pointsMax;
    private int points;
    private int grade;
    @Builder.Default
    private ObjectId Id = new ObjectId();
}

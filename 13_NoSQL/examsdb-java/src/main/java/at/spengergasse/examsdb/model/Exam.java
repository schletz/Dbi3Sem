package at.spengergasse.examsdb.model;

import org.bson.types.ObjectId;
import java.time.LocalDateTime;
import lombok.*;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class Exam {
    private StudentName student;
    private TeacherName teacher;
    private SchoolClass currentClass;
    private SchoolClass examClass;
    private Subject subject;
    private LocalDateTime dateTime;
    private int pointsMax;
    private int points;
    private int grade;
    @Builder.Default
    private ObjectId Id = new ObjectId();
}

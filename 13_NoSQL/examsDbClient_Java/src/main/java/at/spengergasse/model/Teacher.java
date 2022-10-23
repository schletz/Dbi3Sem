package at.spengergasse.model;
import lombok.*;
import org.bson.BsonType;
import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.codecs.pojo.annotations.BsonRepresentation;

import java.math.BigDecimal;
import java.time.LocalTime;
import java.util.ArrayList;
import java.util.HashSet;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class Teacher {
    private TeacherName name;
    private Gender gender;
    private Integer hoursPerWeek;
    private TimeOnly lessonsFrom;
    private BigDecimal salary;
    @Builder.Default
    private HashSet<String> homeOfficeDays = new HashSet<String>();
    @Builder.Default
    private ArrayList<Subject> canTeachSubjects = new ArrayList<Subject>();
    @BsonId
    public String getId() {
        return name.getShortname();
    }
}

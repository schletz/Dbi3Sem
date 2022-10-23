package at.spengergasse.model;
import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

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
    private List<Subject> canTeachSubjects = new ArrayList<Subject>();
    @BsonId
    public String getId() {
        return name.getShortname();
    }
}

package at.spengergasse.examsdb.model;
import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class SchoolClass {
    private String shortname;
    private Term term;
    private String department;
    private int educationLevel;
    private String letter;
    private TeacherName classTeacher;
    private String roomShortname;
    @BsonId
    public String getId() {
        return String.format("%s_%s", term.getId(), shortname);
    }
}

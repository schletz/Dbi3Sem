package at.spengergasse.examsdb.model;
import lombok.*;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class TeacherName {
    private String shortname;
    private String firstname;
    private String lastname;
    private String email;
}

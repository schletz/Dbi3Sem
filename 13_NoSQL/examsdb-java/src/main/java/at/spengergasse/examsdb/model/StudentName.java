package at.spengergasse.examsdb.model;
import lombok.*;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class StudentName {
    public int nr;
    public String firstname;
    public String lastname;
}

package at.spengergasse.examsdb.model;
import lombok.*;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class StudentName {
    public int nr;
    public String firstname;
    public String lastname;
}

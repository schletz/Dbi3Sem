package at.spengergasse.model;
import lombok.*;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class TeacherName {
    private String shortname;
    private String firstname;
    private String lastname;
    private String email;
}

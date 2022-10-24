package at.spengergasse.examsdb.model;
import lombok.*;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Address {
    private String street;
    private String streetNr;
    private String city;
    private int zip;
}

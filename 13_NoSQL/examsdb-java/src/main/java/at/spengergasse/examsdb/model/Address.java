package at.spengergasse.examsdb.model;
import lombok.*;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class Address {
    private String street;
    private String streetNr;
    private String city;
    private int zip;
}

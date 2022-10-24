package at.spengergasse.examsdb.model;

import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

import java.util.List;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Student {
    private StudentName name;
    private Gender gender;
    private Address address;
    private DateOnly dateOfBirth;
    private SchoolClass currentClass;
    private List<SchoolClass> classHistory;

    @BsonId
    public int getId() {
        return name.getNr();
    }
}



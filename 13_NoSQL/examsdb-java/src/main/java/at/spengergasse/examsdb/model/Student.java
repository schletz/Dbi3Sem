package at.spengergasse.examsdb.model;

import java.time.LocalDate;
import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Student {
    private StudentName name;
    private Gender gender;
    private Address address;
    private LocalDate dateOfBirth;
    private SchoolClass currentClass;
    private List<SchoolClass> classHistory;

    @BsonId
    public int getId() {
        return name.getNr();
    }
}



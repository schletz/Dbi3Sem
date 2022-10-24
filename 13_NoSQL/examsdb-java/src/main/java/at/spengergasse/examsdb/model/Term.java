package at.spengergasse.examsdb.model;

import java.time.LocalDate;

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
public class Term {
    private int year;
    private TermType termType;
    private LocalDate start;
    private LocalDate end;

    @BsonId
    public String getId() {
        return String.format("%s%s", String.valueOf(year), termType.toString().charAt(0));
    }
}

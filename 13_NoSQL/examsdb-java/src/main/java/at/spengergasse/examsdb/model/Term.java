package at.spengergasse.examsdb.model;

import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Term {
    private int year;
    private TermType termType;
    private DateOnly start;
    private DateOnly end;

    @BsonId
    public String getId() {
        return String.format("%s%s", String.valueOf(year), termType.toString().charAt(0));
    }
}

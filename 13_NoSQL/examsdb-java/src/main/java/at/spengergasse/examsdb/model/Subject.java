package at.spengergasse.examsdb.model;
import org.bson.codecs.pojo.annotations.BsonId;
import lombok.*;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Subject {
    @BsonId
    private String shortname;
    private String longname;
}

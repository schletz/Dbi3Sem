package at.spengergasse.examsdb.model;
import org.bson.codecs.pojo.annotations.BsonId;
import lombok.*;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class Subject {
    @BsonId
    private String shortname;
    private String longname;
}

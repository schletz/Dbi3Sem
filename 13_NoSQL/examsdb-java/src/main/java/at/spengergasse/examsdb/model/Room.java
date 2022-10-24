package at.spengergasse.examsdb.model;

import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

@Builder
@NoArgsConstructor   // For mongodb codec
@AllArgsConstructor  // For builder
@Getter
@Setter
public class Room {
    @BsonId
    private String shortname;
    private Integer capacity;
}

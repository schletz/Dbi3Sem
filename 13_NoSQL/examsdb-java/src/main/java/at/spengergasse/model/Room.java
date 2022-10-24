package at.spengergasse.model;

import lombok.*;
import org.bson.codecs.pojo.annotations.BsonId;

@Builder
@NoArgsConstructor
@AllArgsConstructor
@Data
public class Room {
    @BsonId
    private String shortname;
    private Integer capacity;
}

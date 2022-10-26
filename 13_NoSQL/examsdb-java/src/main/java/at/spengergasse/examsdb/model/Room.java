package at.spengergasse.examsdb.model;

import org.bson.codecs.pojo.annotations.BsonId;

public record Room(@BsonId String shortname, Integer capacity) {
    public Room {
    }

    public Room(String shortname) {
        this(shortname, null);
    }
}
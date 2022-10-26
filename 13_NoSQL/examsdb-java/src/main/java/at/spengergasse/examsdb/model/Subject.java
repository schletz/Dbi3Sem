package at.spengergasse.examsdb.model;

import org.bson.codecs.pojo.annotations.BsonId;

public record Subject(@BsonId String shortname, String longname) {
}
package at.spengergasse.examsdb.model;

import java.time.LocalDate;

import org.bson.codecs.pojo.annotations.BsonId;


public record Term(int year, TermType termType, LocalDate start, LocalDate end) {
    @BsonId
    public String id() {
        return String.format("%s%s", String.valueOf(year), termType.toString().charAt(0));
    }
}
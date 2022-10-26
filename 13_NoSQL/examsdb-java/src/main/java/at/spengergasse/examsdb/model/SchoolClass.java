package at.spengergasse.examsdb.model;

import org.bson.codecs.pojo.annotations.BsonId;

public record SchoolClass(String shortname, Term term, String department, int educationLevel, String letter,
        TeacherName classTeacher, String roomShortname) {
    @BsonId
    public String id() {
        return String.format("%s_%s", term.id(), shortname);
    }
}
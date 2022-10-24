package at.spengergasse.examsdb.model;

import java.time.LocalTime;
import java.time.format.DateTimeFormatter;

public class TimeOnly {
    private final LocalTime localTime;

    public TimeOnly(int hour, int minute, int second) {
        localTime = LocalTime.of(hour, minute, second);
    }

    private TimeOnly(LocalTime localTime) {
        this.localTime = localTime;
    }

    public LocalTime getLocalTome() {
        return localTime;
    }

    public static TimeOnly parse(String text) {
        return new TimeOnly(LocalTime.parse(text));
    }

    public String format(DateTimeFormatter formatter) {
        return localTime.format(formatter);
    }
}

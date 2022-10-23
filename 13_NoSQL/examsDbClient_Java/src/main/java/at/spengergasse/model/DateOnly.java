package at.spengergasse.model;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;

public class DateOnly {
    private final LocalDate localDate;

    public DateOnly(int year, int month, int day) {
        localDate = LocalDate.of(year, month, day);
    }

    private DateOnly(LocalDate localDate) {
        this.localDate = localDate;
    }

    public LocalDate getLocalDate() {
        return localDate;
    }

    public static DateOnly parse(String text) {
        return new DateOnly(LocalDate.parse(text));
    }

    public String format(DateTimeFormatter formatter) {
        return localDate.format(formatter);
    }
}


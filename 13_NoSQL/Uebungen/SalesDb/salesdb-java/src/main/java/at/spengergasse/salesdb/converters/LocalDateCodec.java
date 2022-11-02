package at.spengergasse.salesdb.converters;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;

import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

/**
 * Reine Datumsangaben sind in der Datenbank als ISO String gespeichert (z. B. 2022-10-24),
 * damit nicht ein Zeitstempel mit UTC Zeitzone verwendet wird. Sonst würde beim Konvertieren
 * in eine lokale Zeit unter Umständen das Datum verändert werden.
 */
public class LocalDateCodec implements Codec<LocalDate> {
    @Override
    public LocalDate decode(BsonReader reader, DecoderContext decoderContext) {
            return LocalDate.parse(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, LocalDate value, EncoderContext encoderContext) {
        writer.writeString(value.format(DateTimeFormatter.ISO_DATE));
    }

    @Override
    public Class<LocalDate> getEncoderClass() {
        return LocalDate.class;
    }
}

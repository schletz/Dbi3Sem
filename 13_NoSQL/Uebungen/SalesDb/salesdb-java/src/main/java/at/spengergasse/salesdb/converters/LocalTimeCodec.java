package at.spengergasse.salesdb.converters;

import java.time.LocalTime;
import java.time.format.DateTimeFormatter;

import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

/**
 * Reine Zeitangaben sind in der Datenbank als String der Form hh:mm gespeichert,
 * damit nicht ein Zeitstempel mit UTC Zeitzone verwendet wird. Sonst würde beim Konvertieren
 * in eine lokale Zeit dieser Wert verändert werden.
 */
public class LocalTimeCodec implements Codec<LocalTime> {
    @Override
    public LocalTime decode(BsonReader reader, DecoderContext decoderContext) {
        return LocalTime.parse(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, LocalTime value, EncoderContext encoderContext) {
        writer.writeString(value.format(DateTimeFormatter.ISO_LOCAL_TIME));
    }

    @Override
    public Class<LocalTime> getEncoderClass() {
        return LocalTime.class;
    }
}

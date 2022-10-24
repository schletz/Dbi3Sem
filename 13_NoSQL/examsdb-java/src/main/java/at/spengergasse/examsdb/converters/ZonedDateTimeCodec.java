package at.spengergasse.examsdb.converters;

import java.time.Instant;
import java.time.ZoneOffset;
import java.time.ZonedDateTime;

import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

/**
 * MongoDB speichert Datumswerte als Millisekunden seit dem 1.1.1970 0:00 UTC. Aus diesem
 * Zeitstempel wird ein ZonedDateTime Objekt erstellt.
 */
public class ZonedDateTimeCodec implements Codec<ZonedDateTime> { 
    @Override
    public ZonedDateTime decode(BsonReader reader, DecoderContext decoderContext) {
        return Instant.ofEpochMilli(reader.readDateTime()).atZone(ZoneOffset.UTC);
    }

    @Override
    public void encode(BsonWriter writer, ZonedDateTime value, EncoderContext encoderContext) {
        writer.writeDateTime(value.toInstant().toEpochMilli());
    }

    @Override
    public Class<ZonedDateTime> getEncoderClass() {
        return ZonedDateTime.class;
    }
}

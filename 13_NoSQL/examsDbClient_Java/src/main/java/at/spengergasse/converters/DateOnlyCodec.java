package at.spengergasse.converters;

import at.spengergasse.model.DateOnly;
import at.spengergasse.model.TimeOnly;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

import java.time.format.DateTimeFormatter;

public class DateOnlyCodec implements Codec<DateOnly> {
    @Override
    public DateOnly decode(BsonReader reader, DecoderContext decoderContext) {
        return DateOnly.parse(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, DateOnly value, EncoderContext encoderContext) {
        writer.writeString(value.format(DateTimeFormatter.ISO_DATE));
    }

    @Override
    public Class<DateOnly> getEncoderClass() {
        return DateOnly.class;
    }
}


package at.spengergasse.converters;

import at.spengergasse.model.TimeOnly;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

import java.time.format.DateTimeFormatter;

public class TimeOnlyCodec implements Codec<TimeOnly> {
    @Override
    public TimeOnly decode(BsonReader reader, DecoderContext decoderContext) {
        return TimeOnly.parse(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, TimeOnly value, EncoderContext encoderContext) {
        writer.writeString(value.format(DateTimeFormatter.ISO_LOCAL_TIME));
    }

    @Override
    public Class<TimeOnly> getEncoderClass() {
        return TimeOnly.class;
    }
}

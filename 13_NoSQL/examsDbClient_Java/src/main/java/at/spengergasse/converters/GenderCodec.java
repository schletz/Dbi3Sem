package at.spengergasse.converters;

import at.spengergasse.model.Gender;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

public class GenderCodec implements Codec<Gender> {
    @Override
    public Gender decode(BsonReader reader, DecoderContext decoderContext) {
        return Gender.valueOf(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, Gender value, EncoderContext encoderContext) {
        writer.writeString(value.name());
    }

    @Override
    public Class<Gender> getEncoderClass() {
        return Gender.class;
    }
}

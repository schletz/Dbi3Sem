package at.spengergasse.examsdb.converters;

import at.spengergasse.examsdb.model.TermType;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;


// See https://www.mongodb.com/docs/drivers/java/sync/current/fundamentals/data-formats/codecs/#std-label-codecs-custom-example
public class TermTypeCodec implements Codec<TermType> {
    @Override
    public TermType decode(BsonReader reader, DecoderContext decoderContext) {
        return TermType.valueOf(reader.readString());
    }

    @Override
    public void encode(BsonWriter writer, TermType value, EncoderContext encoderContext) {
        writer.writeString(value.name());
    }

    @Override
    public java.lang.Class<TermType> getEncoderClass() {
        return TermType.class;
    }
}



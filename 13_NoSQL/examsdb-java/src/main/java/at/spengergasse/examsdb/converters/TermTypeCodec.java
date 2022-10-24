package at.spengergasse.examsdb.converters;

import at.spengergasse.examsdb.model.TermType;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;


/**
 * Speichert die Enumeration TermType als String.
 * Weiterführende Infos (eigener enum Provider und generischer enum Codec) auf
 * https://devpress.csdn.net/mongodb/62f20990c6770329307f5d81.html
 */
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



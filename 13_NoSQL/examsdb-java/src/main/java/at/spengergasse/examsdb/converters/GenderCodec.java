package at.spengergasse.examsdb.converters;

import at.spengergasse.examsdb.model.Gender;
import org.bson.BsonReader;
import org.bson.BsonWriter;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

/**
 * Speichert die Enumeration Gender als String.
 * Weiterführende Infos (eigener enum Provider und generischer enum Codec) auf
 * https://devpress.csdn.net/mongodb/62f20990c6770329307f5d81.html
 */
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

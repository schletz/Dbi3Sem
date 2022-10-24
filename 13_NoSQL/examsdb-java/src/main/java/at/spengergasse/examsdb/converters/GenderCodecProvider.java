package at.spengergasse.examsdb.converters;

import at.spengergasse.examsdb.model.Gender;
import org.bson.codecs.Codec;
import org.bson.codecs.configuration.CodecProvider;
import org.bson.codecs.configuration.CodecRegistry;

public class GenderCodecProvider implements CodecProvider {
    public GenderCodecProvider() {
    }

    @Override
    @SuppressWarnings("unchecked")
    public <T> Codec<T> get(Class<T> clazz, CodecRegistry registry) {
        if (clazz == Gender.class) {
            return (Codec<T>) new GenderCodec();
        }
        // return null when not a provider for the requested class
        return null;
    }
}

package at.spengergasse.converters;

import at.spengergasse.model.DateOnly;
import org.bson.codecs.Codec;
import org.bson.codecs.configuration.CodecProvider;
import org.bson.codecs.configuration.CodecRegistry;

public class DateOnlyCodecProvider implements CodecProvider {
    public DateOnlyCodecProvider() {
    }

    @Override
    @SuppressWarnings("unchecked")
    public <T> Codec<T> get(Class<T> clazz, CodecRegistry registry) {
        if (clazz == DateOnly.class) {
            return (Codec<T>) new DateOnlyCodec();
        }
        // return null when not a provider for the requested class
        return null;
    }
}


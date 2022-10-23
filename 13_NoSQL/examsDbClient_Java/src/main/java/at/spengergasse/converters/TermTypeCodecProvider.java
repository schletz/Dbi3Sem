package at.spengergasse.converters;

import at.spengergasse.model.Gender;
import at.spengergasse.model.TermType;
import org.bson.codecs.Codec;
import org.bson.codecs.configuration.CodecProvider;
import org.bson.codecs.configuration.CodecRegistry;

public class TermTypeCodecProvider implements CodecProvider {
    public TermTypeCodecProvider() {}
    @Override
    @SuppressWarnings("unchecked")
    public <T> Codec<T> get(Class<T> clazz, CodecRegistry registry) {
        if (clazz == TermType.class) {
            return (Codec<T>) new TermTypeCodec();
        }
        // return null when not a provider for the requested class
        return null;
    }
}


package at.spengergasse.examsdb.converters;

import at.spengergasse.examsdb.model.TimeOnly;
import org.bson.codecs.Codec;
import org.bson.codecs.configuration.CodecProvider;
import org.bson.codecs.configuration.CodecRegistry;

public class TimeOnlyCodecProvider implements CodecProvider {
    public TimeOnlyCodecProvider() {
    }

    @Override
    @SuppressWarnings("unchecked")
    public <T> Codec<T> get(Class<T> clazz, CodecRegistry registry) {
        if (clazz == TimeOnly.class) {
            return (Codec<T>) new TimeOnlyCodec();
        }
        // return null when not a provider for the requested class
        return null;
    }
}

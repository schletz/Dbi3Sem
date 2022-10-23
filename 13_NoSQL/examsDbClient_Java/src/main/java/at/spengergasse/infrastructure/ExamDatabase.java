package at.spengergasse.infrastructure;
import at.spengergasse.converters.DateOnlyCodec;
import at.spengergasse.converters.GenderCodec;
import at.spengergasse.converters.TermTypeCodec;
import at.spengergasse.converters.TimeOnlyCodec;
import com.mongodb.ConnectionString;
import com.mongodb.MongoClientSettings;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoDatabase;
import lombok.Getter;
import org.bson.codecs.configuration.CodecRegistries;
import org.bson.codecs.configuration.CodecRegistry;
import org.bson.codecs.pojo.PojoCodecProvider;
import java.util.concurrent.TimeUnit;


@Getter
public class ExamDatabase {
    private final MongoClient client;
    private final MongoDatabase db;

    public static ExamDatabase fromConnectionString(String connectionString)
    {
        var client = MongoClients.create(
                MongoClientSettings.builder()
                        .applyToClusterSettings(builder ->
                                builder.serverSelectionTimeout(5, TimeUnit.SECONDS))
                        .applyConnectionString(new ConnectionString(connectionString))
                        .build());

        CodecRegistry pojoCodecRegistry = CodecRegistries.fromRegistries(
                MongoClientSettings.getDefaultCodecRegistry(),
                CodecRegistries.fromCodecs(new DateOnlyCodec(), new GenderCodec(), new TermTypeCodec(), new TimeOnlyCodec()),
                CodecRegistries.fromProviders(PojoCodecProvider.builder().automatic(true).build()));

        var db = client.getDatabase("examsDb").withCodecRegistry(pojoCodecRegistry);
        return new ExamDatabase(client, db);
    }

    private  ExamDatabase(MongoClient client, MongoDatabase db)
    {
        this.client = client;
        this.db = db;
    }
}

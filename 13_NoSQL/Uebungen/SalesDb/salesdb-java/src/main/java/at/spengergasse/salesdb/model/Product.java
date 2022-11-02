package at.spengergasse.salesdb.model;

import java.math.BigDecimal;
import java.time.ZonedDateTime;

import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.types.ObjectId;

// Collections
public record Product(
                @BsonId ObjectId id, String ean, String name, String category, BigDecimal recommendedPrice,
                int stock, int minStock, ZonedDateTime availableFrom, ZonedDateTime availableTo) {
}
package at.spengergasse.salesdb.model;

import java.time.ZonedDateTime;
import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.types.ObjectId;

public record Order(
                @BsonId ObjectId id, ObjectId customerId, CustomerName customerName, ZonedDateTime dateTime,
                Address shippingAddress, List<OrderItem> orderItems) {
}
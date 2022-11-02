package at.spengergasse.salesdb.model;

import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.types.ObjectId;

public record Customer(
                @BsonId ObjectId id, CustomerName name, Address billingAddress,
                List<Address> shippingAddresses) {
}
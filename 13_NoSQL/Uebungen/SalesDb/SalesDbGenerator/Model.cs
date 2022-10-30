using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SalesDbGenerator
{
    // Value Objects
    record CustomerName(string Firstname, string Lastname, string Email);
    record Address(string Street, string StreetNr, int Zip, string City, string State);
    record OrderItem(
        Product Product,
        int Quantity,
        [property:BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        decimal ItemPrice
    );
    // Collections

    record Product(string Ean, string Name, string Category,
        [property:BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        decimal RecommendedPrice,
        int Stock,
        int MinStock,
        DateTime? AvailableFrom,
        DateTime? AvailableTo,
        ObjectId Id = default
    );
    record Customer(
        CustomerName Name,
        Address BillingAddress,
        ObjectId Id = default
    )
    {
        public List<Address> ShippingAddresses { get; private set; } = new();

    }
    record Order(
        ObjectId CustomerId,
        CustomerName CustomerName,
        DateTime DateTime,
        Address ShippingAddress,
        ObjectId Id = default
    )
    {
        public List<OrderItem> OrderItems { get; private set; } = new();
    }


}

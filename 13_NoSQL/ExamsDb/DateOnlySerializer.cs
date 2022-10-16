using MongoDB.Bson.Serialization;
using System;
/// <summary>
/// Serialisierung von eigenen Typen. In .NET 6 gibt es einen DateOnly Typ. Standardmäßig wird dieser
/// Typ in Mongodb als object mit 3 Feldern geschrieben (Day, Month und Year). Wir erzeugen hier
/// eine Serialisierung als ISO String.
/// Achtung bei Geburtsdaten als DateTime: Es wird die Zeitzone mitgespeichert
/// (z. B: 2000-12-31T00:00:00Z). Beim Konvertieren in die lokale Zeit kann sich der Tag ändern!
/// </summary>
public class DateOnlySerializer : IBsonSerializer<DateOnly>
{
    public Type ValueType => typeof(DateOnly);

    public DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => DateOnly.ParseExact(context.Reader.ReadString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateOnly value)
        => BsonSerializer.Serialize(context.Writer, value.ToString("yyyy-MM-dd"));
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (DateOnly)value);
}

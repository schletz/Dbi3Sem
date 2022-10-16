using MongoDB.Bson.Serialization;
using System;

public class TimeOnlySerializer : IBsonSerializer<TimeOnly>
{
    public Type ValueType => typeof(TimeOnly);

    public TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => TimeOnly.ParseExact(context.Reader.ReadString(), @"HH\:mm", System.Globalization.CultureInfo.InvariantCulture);
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
        => BsonSerializer.Serialize(context.Writer, value.ToString(@"HH\:mm"));
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (DateOnly)value);
}

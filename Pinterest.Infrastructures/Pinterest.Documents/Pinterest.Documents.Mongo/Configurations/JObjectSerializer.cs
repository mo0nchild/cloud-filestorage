using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace Pinterest.Documents.Mongo.Configurations;

internal class JObjectSerializer : SerializerBase<JObject>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
    {
        context.Writer.WriteString(value?.ToString());
    }
    public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var json = context.Reader.ReadString();
        return (string.IsNullOrEmpty(json) ? null : JObject.Parse(json)) ?? throw new InvalidOperationException();
    }
}
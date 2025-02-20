using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using Pinterest.Domain.Core.Models;

namespace Pinterest.Documents.Mongo.Configurations;

public static class MappingConfiguration
{
    public static void ConfigureMappings<TDocument>() where TDocument : BaseEntity
    {
        var conventionPack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("EnumStringConvention", conventionPack, t => true);

        BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(typeof(JObject), new JObjectSerializer());
        
        BsonClassMap.RegisterClassMap<BaseEntity>(mappings =>
        {
            mappings.SetIgnoreExtraElements(true);
            mappings.AutoMap();
            
            mappings.MapIdProperty(document => document.Uuid)
                .SetElementName("uuid")
                .SetSerializer(new GuidSerializer(BsonType.String));
            
            mappings.MapProperty(document => document.CreatedTime)
                .SetElementName("created_at")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        });
        BsonClassMap.RegisterClassMap<TDocument>(mappings =>
        {
            mappings.SetIgnoreExtraElements(true);
            mappings.AutoMap();
        });
    }
}
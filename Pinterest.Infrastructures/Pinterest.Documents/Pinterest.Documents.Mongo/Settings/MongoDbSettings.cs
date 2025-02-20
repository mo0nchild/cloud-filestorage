namespace Pinterest.Documents.Mongo.Settings;

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
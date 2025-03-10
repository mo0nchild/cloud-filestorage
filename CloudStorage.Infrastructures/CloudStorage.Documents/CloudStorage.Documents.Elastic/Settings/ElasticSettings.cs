namespace CloudStorage.Documents.Elastic.Settings;

public class ElasticSettings
{
    public required string ConnectionString { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    
    public string DefaultIndex { get; set; } = "my-index";
}
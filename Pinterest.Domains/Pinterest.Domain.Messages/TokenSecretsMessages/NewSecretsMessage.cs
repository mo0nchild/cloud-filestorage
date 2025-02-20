using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.TokenSecretsMessages;

public class NewSecretsMessage : MessageBase
{
    public static readonly string RoutingPath = "new-secrets";
    
    public string AccessSecret { get; set; } = string.Empty;
    public string RefreshSecret { get; set; } = string.Empty;
    
    public int AccessExpires { get; set; }
    public int RefreshExpires { get; set; }
}
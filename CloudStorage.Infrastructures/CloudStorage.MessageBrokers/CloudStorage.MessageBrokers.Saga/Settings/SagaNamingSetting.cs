namespace CloudStorage.MessageBrokers.Saga.Settings;

public static class SagaNamingSetting
{
    public static string GetExecuteQueueName(string sagaName, string serviceName) => $"{sagaName}-{serviceName}-request";
    
    public static string GetRollbackQueueName(string sagaName, string serviceName) => $"{sagaName}-{serviceName}-rollback";
    
    public static string GetOrchestratorQueueName(string sagaName) => $"{sagaName}-response";
}
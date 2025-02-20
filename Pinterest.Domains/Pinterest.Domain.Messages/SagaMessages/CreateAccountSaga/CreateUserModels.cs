using Pinterest.Domain.Core.Transactions;

namespace Pinterest.Domain.Messages.SagaMessages.CreateAccountSaga;

public static class CreateUserServiceInfo
{
    public static readonly string ServiceName = "UserService";
}

public class CreateUserPayload : SagaPayloadBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateUserCompensation : ISagaRollback
{
    public Guid Uuid { get; set; } = Guid.Empty;
}
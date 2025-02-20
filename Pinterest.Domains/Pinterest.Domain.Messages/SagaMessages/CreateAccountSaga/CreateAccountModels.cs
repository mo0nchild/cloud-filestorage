using Pinterest.Domain.Core.Transactions;

namespace Pinterest.Domain.Messages.SagaMessages.CreateAccountSaga;

public static class CreateAccountServiceInfo
{
    public static readonly string ServiceName = "AccountService";
}

public class CreateAccountPayload : SagaPayloadBase
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateAccountCompensation : ISagaRollback
{
    public string Email { get; set; } = string.Empty;
}
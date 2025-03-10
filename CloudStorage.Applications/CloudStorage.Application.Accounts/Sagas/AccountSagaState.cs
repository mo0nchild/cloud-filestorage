namespace CloudStorage.Application.Accounts.Sagas;

public class AccountSagaState
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
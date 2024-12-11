namespace Pinterest.Domain.Core.Repositories;

public interface IBaseRepository : IDisposable
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public int SaveChanges();
}
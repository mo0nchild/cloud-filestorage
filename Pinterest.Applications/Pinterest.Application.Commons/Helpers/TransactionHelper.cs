using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Commons.Models;

namespace Pinterest.Application.Commons.Helpers;

public class InnerTransactionProcessor
{
    private readonly ConcurrentDictionary<Guid, TransactionState> _transactionStates;
    public InnerTransactionProcessor(ConcurrentDictionary<Guid, TransactionState> transactionStates,
        ILogger<InnerTransactionProcessor> logger)
    {
        Logger = logger;
        _transactionStates = transactionStates;
    }
    private ILogger<InnerTransactionProcessor> Logger { get; }

    public class TransactionResult(Action statusCleaning) : IDisposable
    {
        public void Dispose() => statusCleaning.Invoke();
    }
    public Task<TransactionResult> BeginInnerTransaction(Guid lockUuid)
    {
        if (!_transactionStates.TryGetValue(lockUuid, out TransactionState transactionState)
            || transactionState == TransactionState.Pending)
        {
            throw new ProcessException($"Transaction with Uuid '{lockUuid}' already updating");
        }
        _transactionStates.AddOrUpdate(lockUuid, TransactionState.Pending, (_, _) => TransactionState.Pending);
        return new Task<TransactionResult>(() => new TransactionResult(() =>
        {
            _transactionStates.TryRemove(lockUuid, out _);
        }));
    }
    public Task<TransactionState> GetTransactionState(Guid lockUuid)
    {
        if (!_transactionStates.TryGetValue(lockUuid, out TransactionState transactionState))
        {
            return Task.FromResult(TransactionState.Accepted);
        }
        return Task.FromResult(transactionState);
    }
}
public static class TransactionHelper
{
    public static Task<IServiceCollection> AddInnerTransactionServices(this IServiceCollection serviceCollection, 
        string transactionName)
    {
        serviceCollection.AddKeyedSingleton<ConcurrentDictionary<Guid, TransactionState>>(transactionName);
        serviceCollection.AddKeyedTransient<InnerTransactionProcessor>(transactionName, (provider, _) =>
        {
            var concurrentDictionary = provider.GetRequiredService<ConcurrentDictionary<Guid, TransactionState>>();
            var logger = provider.GetRequiredService<ILogger<InnerTransactionProcessor>>();
            return new InnerTransactionProcessor(concurrentDictionary, logger);
        });
        return Task.FromResult(serviceCollection);
    }
}


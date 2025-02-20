using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Core.Repositories;

namespace Pinterest.Database.Settings.Helpers;

public static class RepositoryFactoryHelper
{
    public static Task<IServiceCollection> AddDbContextFactoryWrapper<TRepository, TContext>(this IServiceCollection collection) 
        where TContext : DbContext, TRepository
        where TRepository : IBaseRepository
    {
        collection.AddTransient<RepositoryFactoryInterface<TRepository>>(services =>
        {
            var factory = services.GetService<IDbContextFactory<TContext>>();
            if(factory == null) throw new NullReferenceException($"{nameof(IDbContextFactory<TContext>)} not found");
            
            return new RepositoryFactoryInterface<TRepository>()
            {
                CreateRepositoryAsync = async () => await factory.CreateDbContextAsync(),
                CreateRepository = () => factory.CreateDbContext()
            };
        });
        return Task.FromResult(collection);
    } 
}
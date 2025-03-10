using MongoDB.Driver;
using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.Core.Repositories;

public interface IDocumentRepository<TDocument> where TDocument : BaseEntity
{
    IMongoCollection<TDocument> Collection { get; }
    
    FilterDefinitionBuilder<TDocument> RepositoryFilter => Builders<TDocument>.Filter;
    UpdateDefinitionBuilder<TDocument> UpdateBuilder => Builders<TDocument>.Update;
}
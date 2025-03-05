using MongoDB.Driver;
using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Core.Repositories;

public interface IDocumentRepository<TDocument> where TDocument : BaseEntity
{
    IMongoCollection<TDocument> Collection { get; }
    
    FilterDefinitionBuilder<TDocument> RepositoryFilter => Builders<TDocument>.Filter;
    UpdateDefinitionBuilder<TDocument> UpdateBuilder => Builders<TDocument>.Update;
}
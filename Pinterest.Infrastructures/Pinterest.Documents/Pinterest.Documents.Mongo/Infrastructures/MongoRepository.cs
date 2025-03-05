using MongoDB.Driver;
using Pinterest.Domain.Core.Models;
using Pinterest.Domain.Core.Repositories;

namespace Pinterest.Documents.Mongo.Infrastructures;

internal class MongoRepository<TDocument> : IDocumentRepository<TDocument>
    where TDocument : BaseEntity
{
    private readonly IMongoDatabase _database;
    private readonly string _collectionName;
    
    public MongoRepository(IMongoClient client, string databaseName, string collectionName)
    {
        _database = client.GetDatabase(databaseName);
        _collectionName = collectionName;
    }
    
    public IMongoCollection<TDocument> Collection => _database.GetCollection<TDocument>(_collectionName);
}
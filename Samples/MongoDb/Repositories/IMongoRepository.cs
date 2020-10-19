using System.Collections.Generic;
using System.Threading.Tasks;
using Joker.AspNetCore.MongoDb.Models;
using MongoDB.Bson;

namespace Joker.AspNetCore.MongoDb.Services
{
  public interface IMongoRepository<TDocument> where TDocument : DomainEntity
  {
    void Initialize();
    Task<List<TDocument>> GetAsync();
    Task<TDocument> GetAsync(string id);
    Task<TDocument> CreateAsync(TDocument document);
    Task UpdateAsync(string id, TDocument document);
    Task ReplaceAsync(string id, TDocument document);
    Task RemoveAsync(TDocument document);
    Task RemoveAsync(string id);
    Task<BsonDocument> GetBuildInfo();
  }
}
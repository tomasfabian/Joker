using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Joker.AspNetCore.MongoDb.Models;
using Joker.AspNetCore.MongoDb.Settings.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Joker.AspNetCore.MongoDb.Services
{
  public abstract class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : DomainEntity
  {
    private readonly IDatabaseSettings databaseSettings;
    private IMongoCollection<TDocument> collection;
    private IMongoDatabase database;
    private MongoClientSettings mongoClientSettings;

    protected MongoRepository(IDatabaseSettings databaseSettings)
    {
      this.databaseSettings = databaseSettings ?? throw new ArgumentNullException(nameof(databaseSettings));
    }

    public void Initialize()
    {
      mongoClientSettings = OnCreateClientSettings();
      
      TrySubscribeToEvents();

      var client = new MongoClient(mongoClientSettings);
      database = client.GetDatabase(databaseSettings.DatabaseName);

      collection = database.GetCollection<TDocument>(databaseSettings.CollectionName);
    }

    protected virtual IEventSubscriber EventSubscriber => default;

    protected IMongoCollection<TDocument> Collection => collection;

    protected virtual MongoClientSettings OnCreateClientSettings()
    {
      return MongoClientSettings.FromUrl(new MongoUrl(databaseSettings.ConnectionString));
    }

    private void TrySubscribeToEvents()
    {
      var eventSubscriber = EventSubscriber;

      if(eventSubscriber != null)
        mongoClientSettings.ClusterConfigurator = builder => builder.Subscribe(eventSubscriber);
    }

    public Task<List<TDocument>> GetAsync() =>
      collection.Find(new BsonDocument()).ToListAsync();

    public Task<TDocument> GetAsync(string id) =>
      collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<TDocument> CreateAsync(TDocument document)
    {
      await collection.InsertOneAsync(document);

      return document;
    }

    public async Task UpdateAsync(string id, TDocument document)
    {
      var updateOptions = new UpdateOptions
      {
        IsUpsert = true
      };

      var update = OnUpdate(document, Builders<TDocument>.Update, updateOptions);

      await collection.UpdateOneAsync(c => c.Id == id, update, updateOptions);
    }

    protected virtual UpdateDefinition<TDocument> OnUpdate(TDocument document, UpdateDefinitionBuilder<TDocument> update, UpdateOptions updateOptions)
    {
      return null;
    }

    public Task ReplaceAsync(string id, TDocument document) =>
      collection.ReplaceOneAsync(c => c.Id == id, document);

    public Task RemoveAsync(TDocument document) =>
      RemoveAsync(document.Id);

    public Task RemoveAsync(string id) => 
      collection.DeleteOneAsync(c => c.Id == id);

    public async Task<BsonDocument> GetBuildInfo()
    {
      var document = new BsonDocument("buildinfo", 1);

      var buildInfo = await database.RunCommandAsync<BsonDocument>(document);

      return buildInfo;
    }
  }
}
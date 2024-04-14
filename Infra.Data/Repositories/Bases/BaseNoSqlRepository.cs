using Domain.Interfaces.Repositories;
using Domain.Models.Bases;
using Domain.Utils.Constants;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infra.Data.Repositories
{
    public class BaseNoSqlRepository<T> : IDisposable, IBaseNoSqlRepository<T> where T : BaseNoSqlModel
    {
        private readonly IMongoClient _mongoClient;
        protected readonly string _collectionName;
        protected readonly IMongoCollection<T> _collection;
        protected readonly FilterDefinitionBuilder<T> _filterBuilder;
        protected IClientSessionHandle? _session;
        protected const string Eq = "$eq";
        protected const string In = "$in";
        protected const string Gte = "$gte";
        protected const string Lte = "$lte";
        protected const string Sort = "$sort";
        protected const string Limit = "$limit";
        protected const string Project = "$project";
        protected const string UnionWith = "$unionWith";
        protected const string Group = "$group";
        protected const string First = "$first";
        protected const string Root = "$$ROOT";
        protected const string ReplaceRoot = "$replaceRoot";
        protected const string Match = "$match";

        public BaseNoSqlRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _collectionName = typeof(T).Name;

            var database = mongoClient.GetDatabase(DbInfo.MongoDbName);
            _collection = database.GetCollection<T>(_collectionName);
            _filterBuilder = Builders<T>.Filter;
        }

        public async Task Save(T modelObject) => await _collection.InsertOneAsync(modelObject);

        public async Task SaveMany(IEnumerable<T> modelList)
        {
            var modelIds = modelList.Select(m => m.Id);
            var filter = Builders<T>.Filter.In(m => m.Id, modelIds);

            await _collection.InsertManyAsync(modelList);
        }

        public async Task Update(T modelObject)
        {
            var filter = Builders<T>.Filter.Eq(m => m.Id, modelObject.Id);

            await _collection.FindOneAndReplaceAsync(filter, modelObject);
        }

        public async Task UpdateMany(IEnumerable<T> modelList)
        {
            var modelIds = modelList.Select(m => m.Id);
            var filter = Builders<T>.Filter.In(m => m.Id, modelIds);

            var updates = new List<WriteModel<T>>();

            foreach (var model in modelList)
            {
                updates.Add(new ReplaceOneModel<T>(filter, model));
            }

            await _collection.BulkWriteAsync(updates, new BulkWriteOptions() { IsOrdered = false });
        }

        public async Task<IEnumerable<T>?> GetAll() => await _collection.Find(c => true).ToListAsync();

        public async Task<T?> GetById(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(m => m.Id, id);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("id", id);

            await _collection.DeleteOneAsync(filter);
        }

        public async Task StartTransaction()
        {
            _session = await _mongoClient.StartSessionAsync();
            _session.StartTransaction();
        }

        public async Task CommitTransaction()
        {
            await _session!.CommitTransactionAsync();
            _session!.Dispose();
        }

        public async Task AbortTransaction()
        {
            await _session!.AbortTransactionAsync();
            _session!.Dispose();
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

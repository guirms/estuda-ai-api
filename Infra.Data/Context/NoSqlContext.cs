using Domain.Utils.Constants;
using MongoDB.Driver;

namespace Infra.Data.Context
{
    public static class NoSqlContext
    {
        public static async Task CreateIndexesAsync<T>(IMongoClient mongoClient, string[] fieldNames)
        {
            try
            {
                var indexBuilder = Builders<T>.IndexKeys.Combine();

                foreach (var field in fieldNames)
                {
                    indexBuilder = indexBuilder.Ascending(field);
                }

                var indexModel = new CreateIndexModel<T>(indexBuilder);

                var database = mongoClient.GetDatabase(DbInfo.MongoDbName);
                var collection = database.GetCollection<T>(typeof(T).Name);

                await collection.Indexes.CreateOneAsync(indexModel);
            }
            catch
            {
                throw new MongoException("Error inserting index into mongodb");
            }
        }
    }
}

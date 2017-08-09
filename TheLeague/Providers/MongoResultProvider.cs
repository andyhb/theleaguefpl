using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoResultProvider : MongoProvider, IMongoResultProvider {

        public MongoResultProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<SharedModels.Result>> GetAll() {
            return await GetAll(FplDataProvider.Season);
        }

        public async Task<List<SharedModels.Result>> GetAll(int season) {
            var collection = _database.GetCollection<SharedModels.Result>("results");
            var filter = Builders<SharedModels.Result>.Filter.Eq("Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderBy(x => x.Id).ToList();
            }

            return new List<SharedModels.Result>();
        }

        public async Task<SharedModels.Result> Get(int id) {
            return await Get(id, FplDataProvider.Season);
        }

        public async Task<SharedModels.Result> Get(int id, int season) {
            var collection = _database.GetCollection<SharedModels.Result>("results");
            var builder = Builders<SharedModels.Result>.Filter;
            var filter = builder.Eq("_id", id) & builder.Eq("Season", season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Result();
        }

        public async void AddResult(SharedModels.Result result) {
            var collection = _database.GetCollection<SharedModels.Result>("results");
            await collection.InsertOneAsync(result);
        }

        public async void UpdateResult(int id, SharedModels.Result result) {
            await UpdateResult(id, FplDataProvider.Season, result);
        }

        public async Task UpdateResult(int id, int season, SharedModels.Result result) {
            var collection = _database.GetCollection<SharedModels.Result>("results");
            var builder = Builders<SharedModels.Result>.Filter;
            var filter = builder.Eq("_id", id) & builder.Eq("Season", season);
            await collection.ReplaceOneAsync(filter, result);
        }
    }
}

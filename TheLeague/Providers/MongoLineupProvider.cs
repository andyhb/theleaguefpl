using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoLineupProvider : MongoProvider, IMongoLineupProvider {

        public MongoLineupProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<SharedModels.Lineup>> GetAll() {
            return await GetAll(FplDataProvider.Season);
        }

        public async Task<List<SharedModels.Lineup>> GetAll(int season) {
            var collection = _database.GetCollection<SharedModels.Lineup>("lineups");
            var filter = Builders<SharedModels.Lineup>.Filter.Eq("Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderBy(x => x.Id).ToList();
            }

            return new List<SharedModels.Lineup>();
        }

        public async Task<SharedModels.Lineup> Get(int id) {
            return await Get(id, FplDataProvider.Season);
        }

        public async Task<SharedModels.Lineup> Get(int id, int season) {
            var collection = _database.GetCollection<SharedModels.Lineup>("lineups");
            var builder = Builders<SharedModels.Lineup>.Filter;
            var filter = builder.Eq("_id", id) & builder.Eq("Season", season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Lineup();
        }

        public async void AddLineup(SharedModels.Lineup lineup) {
            var collection = _database.GetCollection<SharedModels.Lineup>("lineups");
            await collection.InsertOneAsync(lineup);
        }

        public async void UpdateLineup(int id, SharedModels.Lineup lineup) {
            await UpdateLineup(id, FplDataProvider.Season, lineup);
        }

        public async Task UpdateLineup(int id, int season, SharedModels.Lineup lineup) {
            var collection = _database.GetCollection<SharedModels.Lineup>("lineups");
            var builder = Builders<SharedModels.Lineup>.Filter;
            var filter = builder.Eq("_id", id) & builder.Eq("Season", season);
            await collection.ReplaceOneAsync(filter, lineup);
        }
    }
}

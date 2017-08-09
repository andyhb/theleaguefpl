using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoPlayerProvider : MongoProvider, IMongoPlayerProvider {

        public MongoPlayerProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<SharedModels.Player>> GetAll() {
            var collection = _database.GetCollection<SharedModels.Player>("players");
            var documents = await collection.Find(Builders<SharedModels.Player>.Filter.Empty).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.ToList();
            }

            return new List<SharedModels.Player>();
        }

        public async Task<SharedModels.Player> Get(int id) {
            var collection = _database.GetCollection<SharedModels.Player>("players");
            var filter = Builders<SharedModels.Player>.Filter.Eq("_id", id);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Player();
        }

        public async void AddPlayers(List<SharedModels.Player> players) {
            var collection = _database.GetCollection<SharedModels.Player>("players");
            await collection.InsertManyAsync(players);
        }

        public async void UpdatePlayer(SharedModels.Player player) {
            var collection = _database.GetCollection<SharedModels.Player>("players");
            var filter = Builders<SharedModels.Player>.Filter.Eq("_id", player.Id);
            await collection.ReplaceOneAsync(filter, player);
        }

        public async Task<List<SharedModels.Player>> Find(string name) {
            var collection = _database.GetCollection<SharedModels.Player>("players");
            var builder = Builders<SharedModels.Player>.Filter;
            var filter = builder.Regex("SearchName", new BsonRegularExpression(Regex.Escape(name), "i"));

            var document = await collection.Find(filter).ToListAsync();

            if (document != null) {
                return document.OrderBy(x => x.Surname).ThenBy(x => x.Forename).ToList();
            }

            return new List<SharedModels.Player>();
        }

        public async void UpdatePlayerResult(SharedModels.PlayerResult playerResult) {
            var collection = _database.GetCollection<SharedModels.PlayerResult>("player_results");
            await
                collection.ReplaceOneAsync(
                    pr => pr.GameWeek == playerResult.GameWeek && pr.Season == playerResult.Season && pr.PlayerId == playerResult.PlayerId, playerResult,
                    new UpdateOptions {IsUpsert = true});
        }
    }
}

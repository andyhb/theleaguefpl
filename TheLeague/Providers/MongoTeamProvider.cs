using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoTeamProvider : MongoProvider, IMongoTeamProvider {

        public MongoTeamProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<SharedModels.Team>> GetAll() {
            return await GetAll(FplDataProvider.Season);
        }

        public async Task<List<SharedModels.Team>> GetAll(int season) {
            var collection = _database.GetCollection<SharedModels.Team>("teams");
            var filter = Builders<SharedModels.Team>.Filter.Eq("Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderBy(x => x.Id).ToList();
            }

            return new List<SharedModels.Team>();
        }

        public async Task<SharedModels.Team> Get(int id) {
            var collection = _database.GetCollection<SharedModels.Team>("teams");
            var filter = Builders<SharedModels.Team>.Filter.Eq("_id", id);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Team();
        }

        public async Task<SharedModels.Team> GetCurrentTeam(int managerId) {
            var collection = _database.GetCollection<SharedModels.Team>("teams");
            var builder = Builders<SharedModels.Team>.Filter;
            var filter = builder.Eq("Manager", managerId) & builder.Eq("Season", FplDataProvider.Season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Team();
        }

        public void AddTeam(SharedModels.Team team) {
            var collection = _database.GetCollection<SharedModels.Team>("teams");
        }
    }
}

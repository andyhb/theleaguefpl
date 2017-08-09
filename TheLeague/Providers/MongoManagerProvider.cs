using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoManagerProvider : MongoProvider, IMongoManagerProvider {

        public MongoManagerProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<SharedModels.Manager>> GetAll() {
            var collection = _database.GetCollection<SharedModels.Manager>("managers");
            var documents = await collection.Find(Builders<SharedModels.Manager>.Filter.Empty).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.ToList();
            }

            return new List<SharedModels.Manager>();
        }

        public async Task<SharedModels.Manager> Get(int id) {
            var collection = _database.GetCollection<SharedModels.Manager>("managers");
            var filter = Builders<SharedModels.Manager>.Filter.Eq("_id", id);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new SharedModels.Manager();
        }
    }
}

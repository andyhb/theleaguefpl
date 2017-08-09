using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;

namespace TheLeague.Providers {
    public class MongoFplDataRequestProvider : MongoProvider, IMongoFplDataRequestProvider {

        public MongoFplDataRequestProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<SharedModels.FplDataRequest> GetLatest() {
            var collection = _database.GetCollection<SharedModels.FplDataRequest>("fpl_data_requests");
            var document = await collection.Find(Builders<SharedModels.FplDataRequest>.Filter.Empty).ToListAsync();

            if (document != null && document.Count > 0) {
                return document.OrderByDescending(x => x.Id).FirstOrDefault();
            }

            return new SharedModels.FplDataRequest();
        }

        public async void AddDataRequest(SharedModels.FplDataRequest dataRequest) {
            var collection = _database.GetCollection<SharedModels.FplDataRequest>("fpl_data_requests");
            var latest = GetLatest().Result;

            dataRequest.Id = latest.Id + 1;

            await collection.InsertOneAsync(dataRequest);
        }
    }
}

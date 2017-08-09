using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Providers {
    public class MongoTransferProvider : MongoProvider, IMongoTransferProvider {

        public MongoTransferProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<Transfer>> GetAll() {
            return await GetAll(FplDataProvider.Season);
        }

        public async Task<List<Transfer>> GetAll(int season) {
            var collection = _database.GetCollection<Transfer>("transfers");
            var filter = Builders<Transfer>.Filter.Eq("Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderByDescending(x => x.TransferDate).ToList();
            }

            return new List<Transfer>();
        }

        public async Task<List<Transfer>> GetAllForTeam(int teamId) {
            return await GetAllForTeam(teamId, FplDataProvider.Season);
        }

        public async Task<List<Transfer>> GetAllForTeam(int teamId, int season) {
            var collection = _database.GetCollection<Transfer>("transfers");
            var builder = Builders<Transfer>.Filter;
            var filter = (builder.Eq("TeamTransfers.TransferredTo", teamId) | builder.Eq("TeamTransfers.PlayersTransferred.TransferredFrom", teamId)) & builder.Eq("Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderBy(x => x.TransferDate).ToList();
            }

            return new List<Transfer>();
        }

        public async void AddTransfer(Transfer transfer) {
            var collection = _database.GetCollection<Transfer>("transfers");
            await collection.InsertOneAsync(transfer);
        }

        public async void AddPlayersToTeam(TeamTransfer teamTransfer) {
            var collection = _database.GetCollection<Team>("teams");
            var filter = Builders<Team>.Filter.Eq("_id", teamTransfer.TransferredTo);

            var idsToAdd = new List<int>();
            foreach (var playerTransfer in teamTransfer.PlayersTransferred) {
                idsToAdd.Add(playerTransfer.PlayerId);
            }

            var update = Builders<Team>.Update.PushEach("Players", idsToAdd);

            await collection.UpdateOneAsync(filter, update);
        }

        public async void RemovePlayersFromTeam(int teamId, List<int> playersToRemove) {
            var collection = _database.GetCollection<Team>("teams");
            var filter = Builders<Team>.Filter.Eq("_id", teamId);
            var update = Builders<Team>.Update.PullAll("Players", playersToRemove);

            await collection.UpdateOneAsync(filter, update);
        }
    }
}

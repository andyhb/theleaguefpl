using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Providers {
    public class MongoEventProvider : MongoProvider, IMongoEventProvider {
        public MongoEventProvider() {
            _client = new MongoClient(Uri);
            _database = _client.GetDatabase(Database);
        }

        public async Task<List<Event>> GetAll() {
            return await GetAll(FplDataProvider.Season);
        }

        public async Task<List<Event>> GetAll(int season) {
            var collection = _database.GetCollection<Event>("events");
            var filter = Builders<Event>.Filter.Eq("_id.Season", season);

            var documents = await collection.Find(filter).ToListAsync();

            if (documents != null && documents.Any()) {
                return documents.OrderBy(x => x.Id.GameWeek).ToList();
            }

            return new List<Event>();
        }

        public async Task<Event> Get(int id) {
            return await Get(id, FplDataProvider.Season);
        }

        public async Task<Event> Get(int id, int season) {
            var collection = _database.GetCollection<Event>("events");
            var builder = Builders<Event>.Filter;
            var filter = builder.Eq("_id.GameWeek", id) & builder.Eq("_id.Season", season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new Event();
        }

        public async Task<Event> GetCurrentEvent() {
            var collection = _database.GetCollection<Event>("events");
            var builder = Builders<Event>.Filter;
            var filter = builder.Eq("IsCurrent", true) & builder.Eq("_id.Season", FplDataProvider.Season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new Event();
        }

        public async Task<Event> GetNextEvent() {
            var collection = _database.GetCollection<Event>("events");
            var builder = Builders<Event>.Filter;
            var filter = builder.Eq("IsNext", true) & builder.Eq("_id.Season", FplDataProvider.Season);

            var document = await collection.Find(filter).ToListAsync();

            if (document != null && document.Count == 1) {
                return document.FirstOrDefault();
            }

            return new Event();
        }

        public async void AddEvents(List<Event> newEvents) {
            var collection = _database.GetCollection<Event>("events");
            await collection.InsertManyAsync(newEvents);
        }

        public async void UpdateEvent(int id, Event updateEvent) {
            await UpdateEvent(id, FplDataProvider.Season, updateEvent);
        }

        public async Task UpdateEvent(int id, int season, Event updateEvent) {
            var collection = _database.GetCollection<Event>("events");
            var builder = Builders<Event>.Filter;
            var filter = builder.Eq("_id.GameWeek", id) & builder.Eq("_id.Season", season);
            await collection.ReplaceOneAsync(filter, updateEvent);
        }
    }
}

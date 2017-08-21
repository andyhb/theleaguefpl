using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoEventProvider {
        Task<List<SharedModels.Event>> GetAll();
        Task<List<SharedModels.Event>> GetAll(int season);
        Task<SharedModels.Event> Get(int id);
        Task<SharedModels.Event> Get(int id, int season);
        Task<SharedModels.Event> GetCurrentEvent();
        Task<SharedModels.Event> GetNextEvent();
        void AddEvents(List<SharedModels.Event> newEvents);
        void UpdateEvent(int id, SharedModels.Event updateEvent);
        Task UpdateEvent(int id, int season, SharedModels.Event updateEvent);
    }
}
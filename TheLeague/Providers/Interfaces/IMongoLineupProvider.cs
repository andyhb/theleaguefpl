using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoLineupProvider {
        Task<List<SharedModels.Lineup>> GetAll();
        Task<List<SharedModels.Lineup>> GetAll(int season);
        Task<SharedModels.Lineup> Get(int id);
        Task<SharedModels.Lineup> Get(int id, int season);
        void AddLineup(SharedModels.Lineup lineup);
        void UpdateLineup(int id, SharedModels.Lineup lineup);
        Task UpdateLineup(int id, int season, SharedModels.Lineup lineup);
    }
}
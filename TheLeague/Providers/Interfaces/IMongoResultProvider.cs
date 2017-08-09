using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoResultProvider {
        Task<List<SharedModels.Result>> GetAll();
        Task<List<SharedModels.Result>> GetAll(int season);
        Task<SharedModels.Result> Get(int id);
        Task<SharedModels.Result> Get(int id, int season);
        void AddResult(SharedModels.Result result);
        void UpdateResult(int id, SharedModels.Result result);
        Task UpdateResult(int id, int season, SharedModels.Result result);
    }
}
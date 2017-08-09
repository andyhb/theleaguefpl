using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoManagerProvider {
        Task<List<SharedModels.Manager>> GetAll();
        Task<SharedModels.Manager> Get(int id);
    }
}
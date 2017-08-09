using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoPlayerProvider {
        Task<List<SharedModels.Player>> GetAll();
        Task<SharedModels.Player> Get(int id);
        void AddPlayers(List<SharedModels.Player> players);
        void UpdatePlayer(SharedModels.Player player);
        Task<List<SharedModels.Player>> Find(string name);
        void UpdatePlayerResult(SharedModels.PlayerResult playerResult);
    }
}
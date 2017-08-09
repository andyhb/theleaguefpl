using System.Collections.Generic;
using System.Threading.Tasks;
using TheLeague.SharedModels;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoTransferProvider {
        Task<List<Transfer>> GetAll();
        Task<List<Transfer>> GetAll(int season);
        Task<List<Transfer>> GetAllForTeam(int teamId);
        Task<List<Transfer>> GetAllForTeam(int teamId, int season);
        void AddTransfer(Transfer transfer);
        void AddPlayersToTeam(TeamTransfer teamTransfer);
        void RemovePlayersFromTeam(int teamId, List<int> playersToRemove);
    }
}
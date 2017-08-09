using System.Collections.Generic;
using System.Threading.Tasks;
using TheLeague.SharedModels;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoTeamProvider {
        void AddTeam(Team team);
        Task<Team> Get(int id);
        Task<List<Team>> GetAll();
        Task<List<Team>> GetAll(int season);
        Task<Team> GetCurrentTeam(int managerId);
    }
}
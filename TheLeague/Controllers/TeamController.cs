using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class TeamController : Controller {

        private readonly IMongoTeamProvider _mongoTeamProvider;

        public TeamController(IMongoTeamProvider mongoTeamProvider) {
            _mongoTeamProvider = mongoTeamProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Team> GetAllTeams() {
            return _mongoTeamProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public Team GetTeam(int id) {
            return _mongoTeamProvider.Get(id).Result;
        }

        [HttpGet("[action]")]
        public Team GetCurrentTeam(int id) {
            return _mongoTeamProvider.GetCurrentTeam(id).Result;
        }

        [HttpGet("[action]")]
        public Team FindTeamForPlayer(int playerId) {
            var teams = GetAllTeams();

            var foundTeam = new Team {Id = -1};

            foreach (var team in teams) {
                foreach (var teamPlayer in team.Players) {
                    if (teamPlayer == playerId) {
                        foundTeam = team;
                        break;
                    }
                }

                if (foundTeam.Id != -1) {
                    break;
                }
            }

            return foundTeam;
        }
    }
}

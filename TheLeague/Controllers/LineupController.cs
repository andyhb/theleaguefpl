using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class LineupController : Controller {

        private readonly IMongoLineupProvider _mongoLineupProvider;
        private readonly IMongoEventProvider _mongoEventProvider;

        public LineupController(IMongoLineupProvider mongoLineupProvider, IMongoEventProvider mongoEventProvider) {
            _mongoLineupProvider = mongoLineupProvider;
            _mongoEventProvider = mongoEventProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Lineup> GetAllLineups() {
            return _mongoLineupProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public Lineup GetLineup(int id) {
            return _mongoLineupProvider.Get(id).Result;
        }

        [HttpGet("[action]")]
        public TeamLineup GetTeamLineup(int id, int teamId) {
            var lineup = _mongoLineupProvider.Get(id).Result;
            var teamLineup = new TeamLineup(teamId, new List<int>(), DateTime.UtcNow);

            if (lineup.TeamLineups != null && lineup.TeamLineups.Any()) {
                teamLineup = lineup.TeamLineups.FirstOrDefault(x => x.TeamId == teamId);
            }

            return teamLineup;
        }

        [HttpPost("[action]")]
        public TeamLineup AddTeamLineup([FromBody]TeamLineupJson data) {
            var lineup = GetLineup(data.id);
            var teamLineup = new TeamLineup(data.teamId, data.players, DateTime.UtcNow);

            if (lineup?.TeamLineups == null) {
                lineup = new Lineup(data.id, FplDataProvider.Season);
                lineup.TeamLineups.Add(teamLineup);
                _mongoLineupProvider.AddLineup(lineup);
            } else {
                if (lineup.TeamLineups.Count(x => x.TeamId == data.teamId) > 0) {
                    lineup.TeamLineups.FirstOrDefault(x => x.TeamId == data.teamId).Players = data.players;
                    lineup.TeamLineups.FirstOrDefault(x => x.TeamId == data.teamId).DateSet = teamLineup.DateSet;
                } else {
                    lineup.TeamLineups.Add(teamLineup);
                }

                _mongoLineupProvider.UpdateLineup(data.id, lineup);
            }

            return teamLineup;
        }

        public class TeamLineupJson {
            public int id { get; set; }
            public int teamId { get; set; }
            public List<int> players { get; set; }
        }

        [HttpGet("[action]")]
        public int GetCurrentGameWeek() {
            var currentEvent = _mongoEventProvider.GetCurrentEvent().Result;

            if (currentEvent?.Id != null) {
                return currentEvent.Id.GameWeek;
            }

            return 0;
        }
    }
}

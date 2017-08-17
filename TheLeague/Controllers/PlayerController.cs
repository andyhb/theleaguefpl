using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class PlayerController : Controller {

        private readonly IMongoPlayerProvider _mongoPlayerProvider;
        private readonly IMongoTeamProvider _mongoTeamProvider;

        public PlayerController(IMongoPlayerProvider mongoPlayerProvider, IMongoTeamProvider mongoTeamProvider) {
            _mongoPlayerProvider = mongoPlayerProvider;
            _mongoTeamProvider = mongoTeamProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Player> GetAllPlayers() {
            return _mongoPlayerProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public Player GetPlayer(int playerId) {
            return _mongoPlayerProvider.Get(playerId).Result;
        }

        [HttpGet("[action]")]
        public IEnumerable<Player> GetPlayersForTeam(int teamId) {
            var team = _mongoTeamProvider.Get(teamId).Result;

            var teamPlayers = new List<Player>();

            if (team?.Players != null && team.Players.Any()) {
                foreach (var playerId in team.Players) {
                    teamPlayers.Add(GetPlayer(playerId));
                }
            }

            teamPlayers = teamPlayers.OrderBy(x => x.Position).ThenByDescending(x => x.TotalPoints).ToList();

            return teamPlayers;
        }

        [HttpGet("[action]")]
        public IEnumerable<Player> FindPlayer(string name) {
            return _mongoPlayerProvider.Find(name).Result;
        }

        [HttpGet("[action]")]
        public IEnumerable<Player> GetTeamOfTheWeek() {
            var allTeams = _mongoTeamProvider.GetAll().Result;
            var allTeamPlayers = new List<Player>();

            foreach (var team in allTeams) {
                allTeamPlayers.AddRange(GetPlayersForTeam(team.Id));
            }

            var teamOfTheWeek = new List<Player>();

            var goalkeeper = allTeamPlayers.Where(x => x.Position == 1).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).FirstOrDefault();
            teamOfTheWeek.Add(goalkeeper);

            var playersToAdd = new List<Player>();

            var defenders = allTeamPlayers.Where(x => x.Position == 2).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(5).ToList();
            playersToAdd.AddRange(defenders);

            var midfielders = allTeamPlayers.Where(x => x.Position == 3).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(5).ToList();
            playersToAdd.AddRange(midfielders);

            var forwards = allTeamPlayers.Where(x => x.Position == 4).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(3).ToList();
            playersToAdd.AddRange(forwards);

            var highestScoringOutfieldPlayers = GetValidTeamOfWeekPlayers(playersToAdd.OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).ToList());
            teamOfTheWeek.AddRange(highestScoringOutfieldPlayers);

            return teamOfTheWeek.OrderBy(x => x.Position);
        }

        public static readonly Dictionary<int, int> PositionalBounds = new Dictionary<int, int> {
            {2, 5},
            {3, 5},
            {4, 3}
        };

        public List<Player> GetValidTeamOfWeekPlayers(List<Player> players) {
            var playersOfTheWeek = new List<Player>();

            foreach (var player in players) {
                // get number of players of this players position we have already added
                var playersInSamePosition = playersOfTheWeek.Count(x => x.Position == player.Position);

                // if there's still room for more then add this player too
                if (playersInSamePosition < PositionalBounds[player.Position]) {
                    playersOfTheWeek.Add(player);
                }

                // we have enough players so bail
                if (playersOfTheWeek.Count == 10) {
                    break;
                }
            }

            return playersOfTheWeek;
        }
    }
}

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

        public static readonly Dictionary<int, int> PositionalBoundsLower = new Dictionary<int, int> {
            {2, 3},
            {3, 3},
            {4, 1}
        };

        public static readonly Dictionary<int, int> PositionalBoundsUpper = new Dictionary<int, int> {
            {2, 5},
            {3, 5},
            {4, 3}
        };

        private List<Player> GetValidTeamOfWeekPlayers(List<Player> players) {
            var playersOfTheWeek = new List<Player>();

            foreach (var player in players) {
                // get number of players of this players position we have already added
                var playersInSamePosition = playersOfTheWeek.Count(x => x.Position == player.Position);

                // if there's still room for more in this position...
                if (playersInSamePosition < PositionalBoundsUpper[player.Position]) {

                    // and we have enough places left over to still reach valid lower bounds...
                    if (CheckLowerBoundPotential(playersOfTheWeek, player)) {

                        // then add the player
                        playersOfTheWeek.Add(player);
                    }
                    
                }

                // we have enough players so bail
                if (playersOfTheWeek.Count == 10) {
                    break;
                }
            }

            return playersOfTheWeek;
        }

        private bool CheckLowerBoundPotential(List<Player> playersAlreadyAddedRef, Player playerToBeAdded) {
            var playersAlreadyAdded = playersAlreadyAddedRef.ToList();
            playersAlreadyAdded.Add(playerToBeAdded);

            foreach (var position in PositionalBoundsLower) {
                var numberOfPlayersAddedInPosition = playersAlreadyAdded.Count(x => x.Position == position.Key);
                var differenceBetweenNeededAndAlreadyAdded = position.Value - numberOfPlayersAddedInPosition;

                if (differenceBetweenNeededAndAlreadyAdded > (10 - playersAlreadyAdded.Count)) {
                    return false;
                }
            }

            return true;
        }
    }
}

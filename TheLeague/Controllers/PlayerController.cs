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
        public IEnumerable<Player> GetTeamOfTheWeek() {
            // get all players in any team
            var allTeams = _mongoTeamProvider.GetAll().Result;
            var allTeamPlayers = new List<Player>();

            foreach (var team in allTeams) {
                allTeamPlayers.AddRange(GetPlayersForTeam(team.Id));
            }

            var teamOfTheWeek = new List<Player>();

            // get highest scoring goalkeeper on any team
            var goalkeeper = allTeamPlayers.Where(x => x.Position == 1).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).FirstOrDefault();
            teamOfTheWeek.Add(goalkeeper);

            // get four highest scoring defenders on any team (4-0-0)
            var defenders = allTeamPlayers.Where(x => x.Position == 2).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(4).ToList();
            teamOfTheWeek.AddRange(defenders);

            // get highest scoring midfielders and select the minimum of 3 (4-3-0)
            var midfielders = allTeamPlayers.Where(x => x.Position == 3).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(5).ToList();
            var threeMid = midfielders.Take(3);
            var fourthMid = midfielders.Skip(3).FirstOrDefault();
            var fifthMid = midfielders.Skip(4).FirstOrDefault();

            var midfieldersToAdd = new List<Player>();

            teamOfTheWeek.AddRange(threeMid);

            // get highest scoring forwards and select the minimum of 1 (4-3-1)
            var forwards = allTeamPlayers.Where(x => x.Position == 4).OrderByDescending(x => x.RecentPoints).ThenByDescending(x => x.TotalPoints).Take(3).ToList();
            var firstFor = forwards.FirstOrDefault();
            var secondFor = forwards.Skip(1).FirstOrDefault();
            var thirdFor = forwards.Skip(2).FirstOrDefault();

            var forwardsToAdd = new List<Player> { firstFor };

            // determine the rest of the attacking positions

            // add 4th mid (4-4-1)
            if (fourthMid.RecentPoints >= secondFor.RecentPoints) {
                midfieldersToAdd.Add(fourthMid);

                // add 5th mid (4-5-1)
                if (fifthMid.RecentPoints >= secondFor.RecentPoints) {
                    midfieldersToAdd.Add(fifthMid);
                }
            } else {
                // or 2nd forward (4-3-2)
                forwardsToAdd.Add(secondFor);

                // add 4th mid (4-4-2)
                if (fourthMid.RecentPoints >= thirdFor.RecentPoints) {
                    midfieldersToAdd.Add(fourthMid);
                } else {
                    // or 3rd forward (4-3-3)
                    forwardsToAdd.Add(thirdFor);
                }
            }

            teamOfTheWeek.AddRange(midfieldersToAdd);
            teamOfTheWeek.AddRange(forwardsToAdd);

            return teamOfTheWeek;
        }

        [HttpGet("[action]")]
        public IEnumerable<Player> FindPlayer(string name) {
            return _mongoPlayerProvider.Find(name).Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class StatsController : Controller {
        private readonly IMongoResultProvider _mongoResultProvider;

        public StatsController(IMongoResultProvider mongoResultProvider) {
            _mongoResultProvider = mongoResultProvider;
        }

        [HttpGet("[action]")]
        public Stats GetTeamPositions() {
            var allResults = _mongoResultProvider.GetAll().Result;
            var teamScoreTotals = new Dictionary<int, int>();
            var stats = new Stats();

            if (allResults != null) {
                foreach (var result in allResults) {
                    foreach (var teamResult in result.TeamResults) {
                        if (teamScoreTotals.ContainsKey(teamResult.TeamId)) {
                            teamScoreTotals[teamResult.TeamId] += teamResult.Score;
                        } else {
                            teamScoreTotals.Add(teamResult.TeamId, teamResult.Score);
                        }
                    }

                    var orderedScoreTotals = teamScoreTotals.OrderByDescending(x => x.Value).ToList();
                    var gameWeekStat = new GameWeekPosition { GameWeek = result.Id.GameWeek };

                    foreach (var teamResult in result.TeamResults) {
                        

                        for (var x = 0; x < orderedScoreTotals.Count; x++) {
                            var teamIdAtIndex = orderedScoreTotals[x].Key;

                            if (teamIdAtIndex == teamResult.TeamId) {
                                gameWeekStat.TeamPositions.Add(new TeamPosition { Position = x + 1, TeamId = teamResult.TeamId});
                            }
                        }
                    }

                    gameWeekStat.TeamPositions = gameWeekStat.TeamPositions.OrderBy(x => x.TeamId).ToList();

                    stats.GameWeekPositions.Add(gameWeekStat);
                }
            }

            return stats;
        }
    }
}

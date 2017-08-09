using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class ResultController : Controller {

        private readonly IMongoResultProvider _mongoResultProvider;
        private readonly IMongoPlayerProvider _mongoPlayerProvider;
        private readonly IMongoLineupProvider _mongoLineupProvider;

        public ResultController(IMongoResultProvider mongoResultProvider, IMongoPlayerProvider mongoPlayerProvider, IMongoLineupProvider mongoLineupProvider) {
            _mongoResultProvider = mongoResultProvider;
            _mongoPlayerProvider = mongoPlayerProvider;
            _mongoLineupProvider = mongoLineupProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Result> GetAllResults() {
            return _mongoResultProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public IEnumerable<Result> GetRangeOfResults(int start, int end) {
            var results = GetAllResults();
            results = results.Where(x => x.Id >= start && x.Id <= end).ToList();
            return results;
        }

        [HttpGet("[action]")]
        public Result GetResult(int id) {
            return _mongoResultProvider.Get(id).Result;
        }

        [HttpGet("[action]")]
        public TeamResult GetTeamResult(int id, int teamId) {
            var result = _mongoResultProvider.Get(id).Result;
            return result.TeamResults.FirstOrDefault(x => x.TeamId == teamId);
        }

        [HttpPost("[action]")]
        public TeamResult AddTeamResult([FromBody]TeamResultJson data) {
            var result = GetResult(data.id);

            var score = data.score;

            if (data.penalty > 0) {
                var onePercent = (float) score / (float)100;
                score = (int)(onePercent * (float)data.penalty);
            }

            var teamResult = new TeamResult(data.teamId, score, data.penalty);

            if (result?.TeamResults == null) {
                result = new Result(data.id, FplDataProvider.Season);
                result.TeamResults.Add(teamResult);
                _mongoResultProvider.AddResult(result);
            } else {
                if (result.TeamResults.Count(x => x.TeamId == data.teamId) > 0) {
                    result.TeamResults.FirstOrDefault(x => x.TeamId == data.teamId).Score = score;
                    result.TeamResults.FirstOrDefault(x => x.TeamId == data.teamId).Penalty = data.penalty;
                } else {
                    result.TeamResults.Add(teamResult);
                }

                _mongoResultProvider.UpdateResult(data.id, result);
            }

            return teamResult;
        }

        [HttpGet("[action]")]
        public Result UpdateAllGameWeekResultsFromSetLineups(int id) {
            var lineup = _mongoLineupProvider.Get(id).Result;

            var result = GetResult(id) ?? new Result();

            if (lineup != null) {
                foreach (var teamLineup in lineup.TeamLineups) {
                    var teamResult = result.TeamResults.FirstOrDefault(x => x.TeamId == teamLineup.TeamId);

                    var score = 0;

                    foreach (var playerId in teamLineup.Players) {
                        var player = _mongoPlayerProvider.Get(playerId).Result;

                        if (player != null) {
                            score += player.RecentPoints;
                        }
                    }

                    if (teamResult == null) {
                        result.TeamResults.Add(new TeamResult(teamLineup.TeamId, score, 0));
                    } else {
                        teamResult.Score = score;
                    }
                }
            }

            _mongoResultProvider.UpdateResult(id, result);

            return result;
        }

        public class TeamResultJson {
            public int id { get; set; }
            public int teamId { get; set; }
            public int score { get; set; }
            public int penalty { get; set; }
        }

        [HttpGet("[action]")]
        public int GetCurrentGameWeek() {
            return FplDataProvider.GameWeek;
        }
    }
}

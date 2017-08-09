using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using TheLeague.Providers;

namespace TheLeague.SharedModels {
    public class Result {
        /// <summary>
        /// This is the gameweek
        /// </summary>
        [BsonId]
        public int Id;

        public int Season;

        public List<TeamResult> TeamResults;

        public Result() { }

        public Result(int id, int season) {
            Id = id;
            Season = season;
            TeamResults = new List<TeamResult>();
        }
    }

    public class TeamResult {
        public int TeamId;
        public int Score;
        public int Penalty;

        public TeamResult(int id, int score, int penalty) {
            TeamId = id;
            Score = score;
            Penalty = penalty;
        }
    }
}

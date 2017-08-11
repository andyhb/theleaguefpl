using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Result {
        /// <summary>
        /// This is the gameweek
        /// </summary>
        [BsonId]
        public ResultId Id;

        public List<TeamResult> TeamResults;

        public Result() { }

        public Result(int gameWeek, int season) {
            Id = new ResultId(gameWeek, season);
            TeamResults = new List<TeamResult>();
        }
    }

    public class ResultId {
        public int GameWeek;
        public int Season;

        public ResultId(int gameWeek, int season) {
            GameWeek = gameWeek;
            Season = season;
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

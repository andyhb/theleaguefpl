using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using TheLeague.Providers;

namespace TheLeague.SharedModels {
    public class PlayerResult {
        public int GameWeek;
        public int Season;
        public int PlayerId;
        public int Score;

        public PlayerResult(int gameWeek, int season, int playerId, int score) {
            GameWeek = gameWeek;
            Season = season;
            PlayerId = playerId;
            Score = score;
        }
    }
}

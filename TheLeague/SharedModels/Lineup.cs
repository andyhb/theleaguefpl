using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Lineup {
        /// <summary>
        /// This is the gameweek
        /// </summary>
        [BsonId]
        public LineupId Id;

        public List<TeamLineup> TeamLineups;

        public Lineup() { }

        public Lineup(int gameWeek, int season) {
            Id = new LineupId(gameWeek, season);
            TeamLineups = new List<TeamLineup>();
        }
    }

    public class LineupId {
        public int GameWeek;
        public int Season;

        public LineupId(int gameWeek, int season) {
            GameWeek = gameWeek;
            Season = season;
        }
    }

    public class TeamLineup {
        public int TeamId;
        public List<int> Players;
        public DateTime DateSet;

        public TeamLineup(int id, List<int> players, DateTime dateSet) {
            TeamId = id;
            Players = players;
            DateSet = dateSet;
        }
    }
}

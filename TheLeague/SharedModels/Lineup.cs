using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Lineup {
        /// <summary>
        /// This is the gameweek
        /// </summary>
        [BsonId]
        public int Id;

        public int Season;

        public List<TeamLineup> TeamLineups;

        public Lineup() { }

        public Lineup(int id, int season) {
            Id = id;
            Season = season;
            TeamLineups = new List<TeamLineup>();
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

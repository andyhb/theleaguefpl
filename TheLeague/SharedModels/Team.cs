using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Team {
        [BsonId]
        public int Id;

        public string Name;

        public int Manager;

        public int Season;

        public List<int> Players;

        public Team() { }

        public Team(int id, string name, int manager, int season) {
            Id = id;
            Name = name;
            Manager = manager;
            Season = season;
            Players = new List<int>();
        }
    }
}

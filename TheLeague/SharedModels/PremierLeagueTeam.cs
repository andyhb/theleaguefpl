using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class PremierLeagueTeam {
        [BsonId]
        public int Id;

        public string Name;
        public string ShortName;

        public List<EventFixture> CurrentFixtures;
        public List<EventFixture> NextFixtures;

        public PremierLeagueTeam() { }

        public PremierLeagueTeam(int id, string name, string shortName, List<EventFixture> currentFixtures, List<EventFixture> nextFixtures) {
            Id = id;
            Name = name;
            ShortName = shortName;
            CurrentFixtures = currentFixtures;
            NextFixtures = nextFixtures;
        }
    }

    public class EventFixture {
        public int OpponentId;
        public string Opponent;
        public bool Home;

        public EventFixture(int opponentId, string opponent, bool home) {
            OpponentId = opponentId;
            Opponent = opponent;
            Home = home;
        }
    }
}

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Event {
        [BsonId]
        public EventId Id;

        public string Name;

        public bool IsCurrent;
        public bool IsNext;

        public Event() { }

        public Event(int gameWeek, int season, string name, bool isCurrent, bool isNext) {
            Id = new EventId(gameWeek, season);
            Name = name;
            IsCurrent = isCurrent;
            IsNext = isNext;
        }

        public class EventId {
            public int GameWeek;
            public int Season;

            public EventId(int gameWeek, int season) {
                GameWeek = gameWeek;
                Season = season;
            }
        }
    }
}

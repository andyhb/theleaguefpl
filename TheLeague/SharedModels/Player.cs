using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Player {
        [BsonId]
        public int Id;
        public string Forename;
        public string Surname;

        public string FullName {
            get { return Forename + " " + Surname; }
        }

        public string SearchName;
        public string WebName;

        public int TotalPoints;
        public int RecentPoints;

        public int ChanceOfPlayingPercentage;
        public string News;

        public int Position;

        public string TeamName;

        public List<EventFixture> CurrentFixtures;
        public List<EventFixture> NextFixtures;

        public Player() { }

        public Player(int id, string forename, string surname, int totalPoints, int recentPoints, int chanceOfPlayingPercentage, int position, string webname, string news) {
            Id = id;
            Forename = forename;
            Surname = surname;
            TotalPoints = totalPoints;
            RecentPoints = recentPoints;
            ChanceOfPlayingPercentage = chanceOfPlayingPercentage;
            Position = position;
            SearchName = ToAscii(FullName);
            WebName = webname;
            News = news;
        }

        public string ToAscii(string s) {
            return string.Join("",
                s.Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
    }

    public class Positions {
        public const string Goalkeeper = "GK";
        public const string Defender = "DEF";
        public const string Midfielder = "MID";
        public const string Forward = "FOR";
    }
}

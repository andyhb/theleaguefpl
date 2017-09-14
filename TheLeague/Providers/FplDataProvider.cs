using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheLeague.Controllers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Providers {
    public class FplDataProvider {
        private static readonly string _baseUrl = "https://fantasy.premierleague.com/drf/";

        private const string FullDataApi = "bootstrap-static";
        private const string PlayerApi = "element-summary";

        public static int Season = 2;
        public static List<PremierLeagueTeam> PremierLeagueTeamInfo = new List<PremierLeagueTeam>();

        private readonly IMongoEventProvider _mongoEventProvider;
        private readonly IMongoPlayerProvider _mongoPlayerProvider;

        public FplDataProvider(IMongoEventProvider mongoEventProvider, IMongoPlayerProvider mongoPlayerProvider) {
            _mongoEventProvider = mongoEventProvider;
            _mongoPlayerProvider = mongoPlayerProvider;
        }

        public string GetApiLocation(string type) {
            switch (type) {
                case PlayerApi:
                    return _baseUrl + "/" + PlayerApi;
            }

            return _baseUrl + FullDataApi;
        }

        public void GetAllData() {
            using (var httpClient = new HttpClient()) {
                var response = httpClient.GetAsync(GetApiLocation(null)).Result;

                if (response.IsSuccessStatusCode) {

                    dynamic data = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                    GetEvents(data.events);
                    GetTeams(data.teams);
                    GetPlayers(data.elements);
                }
            }
        }

        public void GetEvents(dynamic data) {
            var storedEvents = _mongoEventProvider.GetAll().Result;
            var newEvents = new List<Event>();

            foreach (var fplEvent in data) {
                var newEvent = new Event((int)fplEvent.id, Season, fplEvent.name.ToString(), (bool)fplEvent.is_current, (bool)fplEvent.is_next);

                var storedEvent = storedEvents.FirstOrDefault(x => x.Id.GameWeek == newEvent.Id.GameWeek && x.Id.Season == newEvent.Id.Season);

                if (storedEvent != null) {
                    if (storedEvent.Name != newEvent.Name || storedEvent.IsCurrent != newEvent.IsCurrent ||
                        storedEvent.IsNext != newEvent.IsCurrent) {
                        _mongoEventProvider.UpdateEvent(storedEvent.Id.GameWeek, storedEvent.Id.Season, newEvent);
                    }
                } else {
                    newEvents.Add(newEvent);
                }
            }

            if (newEvents.Any()) {
                _mongoEventProvider.AddEvents(newEvents);
            }
        }

        public void GetTeams(dynamic data) {
            foreach (var team in data) {
                // get current fixtures
                var currentFixtures = new List<EventFixture>();
                foreach (var currentEvent in team.current_event_fixture) {
                    currentFixtures.Add(new EventFixture((int) currentEvent.opponent, string.Empty,
                        bool.Parse(currentEvent.is_home.ToString())));
                }

                // get next fixtures
                var nextFixtures = new List<EventFixture>();
                foreach (var nextEvent in team.next_event_fixture) {
                    nextFixtures.Add(new EventFixture((int) nextEvent.opponent, string.Empty,
                        bool.Parse(nextEvent.is_home.ToString())));
                }

                // populate team
                PremierLeagueTeamInfo.Add(new PremierLeagueTeam((int) team.id, team.name.ToString(), team.short_name.ToString(), currentFixtures, nextFixtures));
            }

            foreach (var premierLeagueTeam in PremierLeagueTeamInfo) {
                foreach (var currentFixture in premierLeagueTeam.CurrentFixtures) {
                    currentFixture.Opponent =
                        PremierLeagueTeamInfo.FirstOrDefault(x => x.Id == currentFixture.OpponentId)?.ShortName;
                }

                foreach (var nextFixture in premierLeagueTeam.NextFixtures) {
                    nextFixture.Opponent =
                        PremierLeagueTeamInfo.FirstOrDefault(x => x.Id == nextFixture.OpponentId)?.ShortName;
                }
            }
        }

        public void GetPlayers(dynamic data) {
            var storedPlayers = _mongoPlayerProvider.GetAll().Result;
            var newPlayers = new List<Player>();

            foreach (var player in data) {

                var updatePlayer = new Player((int) player.id, player.first_name.ToString(),
                    player.second_name.ToString(), (int) player.total_points, (int) player.event_points,
                    player.chance_of_playing_next_round != null ? (int) player.chance_of_playing_next_round : -1,
                    (int) player.element_type, player.web_name.ToString(), player.news.ToString());

                var playersTeam = PremierLeagueTeamInfo.FirstOrDefault(x => x.Id == (int) player.team);
                if (playersTeam != null) {
                    updatePlayer.CurrentFixtures = playersTeam.CurrentFixtures;
                    updatePlayer.NextFixtures = playersTeam.NextFixtures;
                    updatePlayer.TeamName = playersTeam.ShortName;
                }

                var storedPlayer = storedPlayers.FirstOrDefault(x => x.Id == updatePlayer.Id);

                if (storedPlayer != null) {
                    if (updatePlayer.TotalPoints != storedPlayer.TotalPoints ||
                        updatePlayer.RecentPoints != storedPlayer.RecentPoints ||
                        updatePlayer.ChanceOfPlayingPercentage != storedPlayer.ChanceOfPlayingPercentage ||
                        updatePlayer.Surname != storedPlayer.Surname ||
                        updatePlayer.NextFixtures != storedPlayer.NextFixtures ||
                        updatePlayer.WebName != storedPlayer.WebName ||
                        updatePlayer.News != storedPlayer.News) {

                        _mongoPlayerProvider.UpdatePlayer(updatePlayer);
                    }
                } else {
                    newPlayers.Add(updatePlayer);
                }
            }

            if (newPlayers.Any()) {
                _mongoPlayerProvider.AddPlayers(newPlayers);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheLeague.Controllers;
using TheLeague.SharedModels;

namespace TheLeague.Providers {
    public class FplDataProvider {
        private static readonly string _baseUrl = "https://fantasy.premierleague.com/drf/";

        private const string FullDataApi = "bootstrap-static";
        private const string PlayerApi = "element-summary";

        public static int GameWeek;
        public static int Season = 2;
        public static List<PremierLeagueTeam> PremierLeagueTeamInfo = new List<PremierLeagueTeam>();

        public static string GetApiLocation(string type) {
            switch (type) {
                case PlayerApi:
                    return _baseUrl + "/" + PlayerApi;
            }

            return _baseUrl + FullDataApi;
        }

        public static void GetAllData() {
            using (var httpClient = new HttpClient()) {
                var response = httpClient.GetAsync(GetApiLocation(null)).Result;
                dynamic data = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                SetGameWeek(data.events);
                GetTeams(data.teams);
                GetPlayers(data.elements);

                AddDataRequest();
            }
        }

        public static void SetGameWeek(dynamic data) {
            foreach (var fplEvent in data) {
                var isCurrent = (bool) fplEvent.is_current;
                var id = (int) fplEvent.id;

                if (isCurrent) {
                    GameWeek = id;
                    break;
                }
            }
        }

        public static void GetTeams(dynamic data) {
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

        public static void GetPlayers(dynamic data) {
            var playersProvider = new MongoPlayerProvider();
            var storedPlayers = playersProvider.GetAll().Result;
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

                        playersProvider.UpdatePlayer(updatePlayer);
                    }

                    //if (GameWeek > 0) {
                        //playersProvider.UpdatePlayerResult(new PlayerResult(GameWeek, Season, updatePlayer.Id,
                            //updatePlayer.RecentPoints));
                    //}
                } else {
                    newPlayers.Add(updatePlayer);
                }
            }

            if (newPlayers.Any()) {
                playersProvider.AddPlayers(newPlayers);
            }
        }

        public static void AddDataRequest() {
            var dataProvider = new MongoFplDataRequestProvider();
            var dataRequest = new FplDataRequest() {
                RequestDate = DateTime.UtcNow
            };

            dataProvider.AddDataRequest(dataRequest);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLeague.SharedModels {
    public class Stats {
        public List<GameWeekPosition> GameWeekPositions;

        public Stats() {
            GameWeekPositions = new List<GameWeekPosition>();
        }
    }

    public class GameWeekPosition {
        public int GameWeek;
        public List<TeamPosition> TeamPositions;

        public GameWeekPosition() {
            TeamPositions = new List<TeamPosition>();
        }
    }

    public class TeamPosition {
        public int Position;
        public int TeamId;
    }
}

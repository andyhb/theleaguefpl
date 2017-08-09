using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TheLeague.SharedModels {
    public class Transfer {
        [BsonId]
        public Guid Id;

        public int Season;
        public List<TeamTransfer> TeamTransfers;

        public DateTime TransferDate;

        public Transfer(List<TeamTransfer> teamTransfers, DateTime transferDate, int season) {
            Id = Guid.NewGuid();
            TeamTransfers = teamTransfers;
            TransferDate = transferDate;
            Season = season;
        }
    }

    public class TeamTransfer {
        public int TransferredTo;
        public List<PlayersTransferred> PlayersTransferred;

        public TeamTransfer(int transferredTo, List<PlayersTransferred> playersTransferred) {
            TransferredTo = transferredTo;
            PlayersTransferred = playersTransferred;
        }
    }

    public class PlayersTransferred {
        public int PlayerId;
        public int TransferredFrom;

        public PlayersTransferred(int playerId, int trasferredFrom) {
            PlayerId = playerId;
            TransferredFrom = trasferredFrom;
        }
    }
}

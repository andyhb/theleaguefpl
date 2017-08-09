using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class TransferController : Controller {

        private readonly IMongoTransferProvider _mongoTransferProvider;

        public TransferController(IMongoTransferProvider mongoTransferProvider) {
            _mongoTransferProvider = mongoTransferProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Transfer> GetAllTransfers() {
            return _mongoTransferProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public IEnumerable<Transfer> GetTransfersForTeam(int teamId) {
            return _mongoTransferProvider.GetAllForTeam(teamId).Result;
        }

        [HttpPost("[action]")]
        public Transfer MakeTransfer([FromBody]TransferJson data) {
            // add the transfer
            var transfer = ConvertToTransferFromJson(data);
            _mongoTransferProvider.AddTransfer(transfer);

            // add the players being transferred in to the new team
            foreach (var teamTransfer in transfer.TeamTransfers) {
                if (teamTransfer.TransferredTo != -1) {
                    _mongoTransferProvider.AddPlayersToTeam(teamTransfer);
                }
            }

            // remove the players from the old teams
            var playersToRemove = new Dictionary<int, List<int>>();
            foreach (var teamTransfer in transfer.TeamTransfers) {
                foreach (var playerTransfer in teamTransfer.PlayersTransferred) {
                    if (playerTransfer.TransferredFrom != -1) {
                        if (playersToRemove.ContainsKey(playerTransfer.TransferredFrom)) {
                            playersToRemove[playerTransfer.TransferredFrom].Add(playerTransfer.PlayerId);
                        } else {
                            playersToRemove.Add(playerTransfer.TransferredFrom, new List<int>() {playerTransfer.PlayerId});
                        }
                    }
                }
            }

            foreach (var toRemove in playersToRemove) {
                _mongoTransferProvider.RemovePlayersFromTeam(toRemove.Key, toRemove.Value);
            }

            // return the transfer details
            return transfer;
        }

        public Transfer ConvertToTransferFromJson(TransferJson transferJson) {
            var transfer = new Transfer(new List<TeamTransfer>(), DateTime.UtcNow, FplDataProvider.Season);

            foreach (var teamTransferJson in transferJson.teamTransfers) {
                var teamTransfer = new TeamTransfer(teamTransferJson.transferredTo, new List<PlayersTransferred>());

                foreach (var playersTransferedJson in teamTransferJson.playersTransferred) {
                    var playerTransferred = new PlayersTransferred(playersTransferedJson.playerId, playersTransferedJson.transferredFrom);
                    teamTransfer.PlayersTransferred.Add(playerTransferred);
                }

                if (transfer.TeamTransfers.Select(x => x.TransferredTo).Contains(teamTransfer.TransferredTo)) {
                    transfer.TeamTransfers.FirstOrDefault(x => x.TransferredTo == teamTransfer.TransferredTo).PlayersTransferred.AddRange(teamTransfer.PlayersTransferred);
                } else {
                    transfer.TeamTransfers.Add(teamTransfer);
                }
            }

            return transfer;
        }

        public class TransferJson {
            public List<TeamTransferJson> teamTransfers { get; set; }
        }

        public class TeamTransferJson {
            public List<PlayersTransferedJson> playersTransferred { get; set; }
            public int transferredTo { get; set; }
        }

        public class PlayersTransferedJson {
            public int playerId { get; set; }
            public int transferredFrom { get; set; }
        }
    }
}

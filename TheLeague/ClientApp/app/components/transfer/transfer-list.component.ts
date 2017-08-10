import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';

import { Transfer, TeamTransfer, PlayerTransferred, PlayerTransferredItem, FullTransferItem } from "../../models/transfer";
import TransferService = require("../../services/transfer.service");
import TeamService = require("../../services/team.service");
import PlayerService = require("../../services/player.service");
import AuthService = require("../../services/auth.service");

@Component({
    selector: 'transfer-list',
    template: require('./transfer-list.component.html'),
    styles: [require('./transfer-list.component.css')]
})

export class TransferListComponent implements OnInit {
    fullTransfers: FullTransferItem[] = [];

    constructor(private transferService: TransferService.TransferService, private teamService: TeamService.TeamService, private playerService: PlayerService.PlayerService, private auth: AuthService.Auth) { }

    ngOnInit(): void {
        this.transferService.getAllTransfers()
            .subscribe(
                transfers => {
                    transfers.forEach(transfer => {
                        let fullTransferItem = new FullTransferItem();

                        fullTransferItem.transferDate = transfer.transferDate;
                        fullTransferItem.playersTransferred = [];

                        if (transfer.teamTransfers && transfer.teamTransfers.length > 0) {
                            transfer.teamTransfers.forEach(teamTransfer => {

                                if (teamTransfer.playersTransferred && teamTransfer.playersTransferred.length > 0) {
                                    teamTransfer.playersTransferred.forEach(playerTransfer => {
                                        let playerTransferItem = new PlayerTransferredItem();

                                        if (teamTransfer.transferredTo === -1) {
                                            playerTransferItem.transferredToTeamName = "Unattached";
                                        } else {
                                            this.teamService.getTeam(teamTransfer.transferredTo)
                                                .subscribe(transferredToTeam => {
                                                    playerTransferItem.transferredToTeamName = transferredToTeam.name;
                                                });
                                        }

                                        if (playerTransfer.transferredFrom === -1) {
                                            playerTransferItem.transferredFromTeamName = "Unattached";
                                        } else {
                                            this.teamService.getTeam(playerTransfer.transferredFrom)
                                                .subscribe(transferredFromTeam => {
                                                    playerTransferItem
                                                        .transferredFromTeamName = transferredFromTeam.name;
                                                });
                                        }

                                        this.playerService.getPlayer(playerTransfer.playerId)
                                            .subscribe(transferredPlayer => {
                                                playerTransferItem.playerName = transferredPlayer.fullName;
                                            });

                                        fullTransferItem.playersTransferred.push(playerTransferItem);
                                    });
                                }
                            });
                        }

                        this.fullTransfers.push(fullTransferItem);
                    });
                }
            );
    }
}

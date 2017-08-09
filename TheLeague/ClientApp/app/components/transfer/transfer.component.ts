import { Component, Input, OnInit, OnChanges, OnDestroy } from '@angular/core';

import { Player } from "../../models/player";
import { Team } from "../../models/team";
import { Transfer, TeamTransfer, PlayerTransferred } from "../../models/transfer";

import PlayerService = require("../../services/player.service");
import TransferService = require("../../services/transfer.service");
import TeamService = require("../../services/team.service");
import AuthService = require("../../services/auth.service");

@Component({
    selector: 'transfer',
    template: require('./transfer.component.html'),
    styles: [require('./transfer.component.css')]
})

export class TransferComponent implements OnInit, OnDestroy {
    @Input() initialPlayerToTransfer: Player;
    @Input() searchPlayerName: string;
    teams: Team[];

    transfers: FullTransferRowItem[] = [];
    completedTransfer: Transfer;

    playersAdded: Player[];

    fullInitialTeam: Team;

    constructor(private playerService: PlayerService.PlayerService,
        private auth: AuthService.Auth,
        private transferService: TransferService.TransferService,
        private teamService: TeamService.TeamService) { }

    ngOnDestroy(): void {
        this.transferService.playersAdded = [];
        this.transferService.initialPlayerToTranser = null;
    }

    ngOnInit(): void {
        this.teamService.getTeams()
            .subscribe(teams => this.teams = teams);

        if (this.transferService.initialPlayerToTranser) {
            this.initialPlayerToTransfer = this.transferService.initialPlayerToTranser;
        }

        let self = this;

        this.transferService.playersChange
            .subscribe(playersAdded => {
                this.playersAdded = playersAdded;

                let existingPlayers = self.transfers.map(transfer => transfer.player);

                this.playersAdded.forEach(playerAdded => {
                    let fullTransferRow = self.getFullTransferRow(playerAdded);
                    
                    if (fullTransferRow && existingPlayers.indexOf(playerAdded) === -1) {
                        self.transfers.push(fullTransferRow);
                    }
                });
            });

        if (this.initialPlayerToTransfer) {
            let fullInitial = this.getFullTransferRow(this.initialPlayerToTransfer);

            if (fullInitial) {
                this.transfers.push(fullInitial);
            }
        }
    }

    getFullTransferRow(player: Player): FullTransferRowItem {
        let fullTransferRow = new FullTransferRowItem();

        fullTransferRow.player = player;

        this.teamService.findTeamForPlayer(player.id)
            .subscribe(team => {
                if (team && team.id !== -1) {
                    fullTransferRow.fromTeam = team;
                }
            });

        return fullTransferRow;
    }

    teamAllocated(team: Team): boolean {
        if (team && team.id !== -1) {
            return true;
        }

        return false;
    }

    changeTransferToTeam(transfer: FullTransferRowItem, teamId: number): void {
        transfer.toTeam = this.teams.filter(team => (team.id === Number(teamId)))[0];
    }

    completeTransfer(): void {
        let teamTransfers: TeamTransfer[] = [];

        this.transfers.forEach(transferRow => {
            let teamTransfer = new TeamTransfer;

            if (transferRow.toTeam) {
                teamTransfer.transferredTo = transferRow.toTeam.id;
            } else {
                teamTransfer.transferredTo = -1;
            }

            teamTransfer.playersTransferred = [];

            if (teamTransfers.indexOf(teamTransfer) === -1) {
                teamTransfers.push(teamTransfer);
            }

            let playerTransferred = new PlayerTransferred;
            playerTransferred.playerId = transferRow.player.id;

            if (transferRow.fromTeam) {
                playerTransferred.transferredFrom = transferRow.fromTeam.id;
            } else {
                playerTransferred.transferredFrom = -1;
            }

            teamTransfers[teamTransfers.indexOf(teamTransfer)].playersTransferred.push(playerTransferred);
        });

        this.transferService.makeTransfer(teamTransfers)
            .subscribe(
                transferResult => this.completedTransfer = transferResult
            );
    }

    searchForPlayer(name: string): void {
        this.playerService.changeSearchParam(name);
    }
}

class FullTransferRowItem {
    player: Player;
    fromTeam: Team;
    toTeam: Team;
}
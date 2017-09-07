import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { Router } from '@angular/router';

import { Player, EventFixture } from "../../models/player";
import { TeamResult } from "../../models/result";
import { TeamLineup } from "../../models/lineup";

import PlayerService = require("../../services/player.service");
import AuthService = require("../../services/auth.service");
import ResultService = require("../../services/result.service");
import LineupService = require("../../services/lineup.service");
import TransferService = require("../../services/transfer.service");

@Component({
    selector: 'players',
    template: require('./player-list.component.html'),
    styles: [require('./player-list.component.css')]
})

export class PlayersComponent implements OnInit {
    pageTitle: string = "Team of the week";

    @Input() teamId: number;
    @Input() lineupDetails: boolean;
    @Input() playerSelection: boolean;
    @Input() teamSelection: boolean;
    @Input() penalty: number = 0;

    @Input() transferSearch: boolean;
    searchParam: string;

    players: Player[];
    selectedPlayers: Player[] = [];
    currentGameWeekLineupPlayers: Player[] = [];

    validFormations: string[] = [
        "3-4-3",
        "3-5-2",
        "4-3-3",
        "4-4-2",
        "4-5-1",
        "5-3-2",
        "5-4-1"
    ];

    currentGameWeek: number;
    teamResult: TeamResult;
    teamLineup: TeamLineup;

    successfulNotification: string;

    constructor(private playerService: PlayerService.PlayerService,
        private auth: AuthService.Auth,
        private resultService: ResultService.ResultService,
        private lineupService: LineupService.LineupService,
        private transferService: TransferService.TransferService,
        private router: Router) { }

    ngOnInit(): void {
        this.selectedPlayers = [];

        this.resultService.getCurrentGameWeek()
            .subscribe(
                currentGameWeek => {
                    this.currentGameWeek = currentGameWeek;

                    if (this.transferSearch) {
                        this.pageTitle = "Search for players to transfer";

                        this.searchParam = this.playerService.searchParam;

                        this.playerService.searchParamChange
                            .subscribe(searchParam => {
                                this.searchParam = searchParam;
                                this.getTransferPlayers();
                            });

                        this.getTransferPlayers();
                    } else if (this.teamId || this.teamId === 0) {
                        this.pageTitle = "Click on a player to toggle their selection";

                        this.getPlayersForTeam(this.teamId);
                    } else {
                        // otherwise get the team of the week list
                        this.getTeamOfTheWeek();
                    }
                }
            );
    }

    ngOnChanges(changes): void {
        if (changes.teamId) {
            this.selectedPlayers = [];
            this.getPlayersForTeam(this.teamId);
        }
    }

    getTransferPlayers() {
        this.playerService.findPlayer(this.searchParam)
            .subscribe(players => this.players = players);
    }

    getPlayersForTeam(teamId: number) {
        this.playerService.getPlayersForTeam(teamId)
            .subscribe(
            players => {
                this.players = players;

                if (this.teamSelection) {
                    this.lineupService.getGameWeekLineupForTeam(this.currentGameWeek + 1, teamId)
                        .subscribe(
                        teamLineup => {
                            this.teamLineup = teamLineup;
                            this.selectedPlayers = this.players
                                .filter(player => (teamLineup.players.indexOf(player.id) > -1));
                        }
                        );
                }

            }
            );
    }

    getTeamOfTheWeek() {
        this.playerService.getTeamOfTheWeek()
            .subscribe(
            players => this.players = players
            );
    }

    togglePlayerSelection(player: Player): void {
        if (!this.playerSelection) {
            // check if doing transfer
            if (this.transferSearch) {
                this.transferService.addPlayerToTransfer(player);
            }

            return;
        }

        if (this.selectedPlayers.indexOf(player) === -1) {
            if (this.selectedPlayers.length < 11) {
                this.selectedPlayers.push(player);
            }
        } else {
            this.selectedPlayers.splice(this.selectedPlayers.indexOf(player), 1);
        }

        this.getNumberOfPlayers();
    }

    isSelected(player: Player): boolean {
        if (!this.playerSelection) {
            return false;
        }

        if (this.selectedPlayers.indexOf(player) > -1) {
            return true;
        }

        return false;
    }

    isValidFormation(): boolean {
        var formation = this.getFormationString();

        if (this.validFormations.indexOf(formation) > -1) {
            return true;
        }

        return false;
    }

    getFormationString(): string {
        var def = 0, mid = 0, str = 0;

        var playersForFormation = this.players;

        if (this.playerSelection) {
            playersForFormation = this.selectedPlayers;
        }

        playersForFormation.forEach(player => {
            if (player.position === 2) {
                def++;
            }

            if (player.position === 3) {
                mid++;
            }

            if (player.position === 4) {
                str++;
            }
        });

        return def + "-" + mid + "-" + str;
    }

    getTeamTotal(): number {
        var total = 0;

        var playersForTotal = this.players;

        if (this.playerSelection) {
            playersForTotal = this.selectedPlayers;
        }

        playersForTotal.forEach(player => {
            total += player.recentPoints;
        });

        return total;
    }

    getNumberOfPlayers(): string {
        if (!this.playerSelection) {
            return "";
        }

        return this.selectedPlayers.length + "";
    }

    addResult(): void {
        this.resultService.addResult(this.currentGameWeek, this.teamId, this.getTeamTotal(), this.penalty)
            .subscribe(
            teamResult => this.teamResult = teamResult
            );
    }

    addLineup(): void {
        if (this.selectedPlayers && this.selectedPlayers.length === 11) {
            let selectedPlayerIds = this.selectedPlayers.map(player => player.id);

            this.lineupService.addLineup(this.currentGameWeek + 1, this.teamId, selectedPlayerIds)
                .subscribe(
                    teamLineup => {
                        this.teamLineup = teamLineup;

                        this.successfulNotification = this.teamLineup.dateSet;
                    }
                );
        }
    }

    transferOut(player: Player): void {
        this.transferService.initialPlayerToTranser = player;
        this.router.navigate(['transfer']);
    }

    populateFromLineup(): void {
        this.getLineup(this.currentGameWeek);
    }

    populateFromNextGameWeekLineup(): void {
        this.getLineup(this.currentGameWeek + 1);
    }

    getLineup(gameweek: number): void {
        this.lineupService.getGameWeekLineupForTeam(gameweek, this.teamId)
            .subscribe(
                selectedPlayers => {
                    this.selectedPlayers = this.players
                        .filter(player => (selectedPlayers.players.indexOf(player.id) > -1));
                }
            );
    }
}
import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';

import { Team } from "../../models/team";
import { Lineup, TeamLineup, TeamLineupItem, TeamLineupPlayer } from "../../models/lineup";

import TeamService = require("../../services/team.service");
import LineupService = require("../../services/lineup.service");
import ResultService = require("../../services/result.service");
import PlayerService = require("../../services/player.service");

@Component({
    selector: 'lineups',
    template: require('./lineups.component.html'),
    styles: [require('./lineups.component.css')]
})

export class LineupsComponent implements OnInit {
    gameWeekLineups: Lineup;
    nextGameWeek: number;
    teams: Team[];
    teamLineups: TeamLineupItem[] = [];

    constructor(private teamService: TeamService.TeamService,
        private lineupService: LineupService.LineupService,
        private resultService: ResultService.ResultService,
        private playerService: PlayerService.PlayerService) { }

    ngOnInit(): void {
        this.resultService.getCurrentGameWeek()
            .subscribe(currentGameWeek => {
                this.nextGameWeek = currentGameWeek + 1;

                this.teamService.getTeams()
                    .subscribe(teams => {
                        this.teams = teams;

                        this.lineupService.getGameWeekLineup(this.nextGameWeek)
                            .subscribe(lineup => {
                                this.gameWeekLineups = lineup;

                                if (this.gameWeekLineups &&
                                    this.gameWeekLineups.teamLineups &&
                                    this.gameWeekLineups.teamLineups.length > 0) {
                                    this.teams.forEach(team => {
                                        let teamLineupItem = new TeamLineupItem();
                                        teamLineupItem.teamName = team.name;
                                        teamLineupItem.players = [];

                                        let teamLineups = this.gameWeekLineups.teamLineups.filter(teamLineup => {
                                            return team.id === teamLineup.teamId;
                                        });

                                        if (teamLineups && teamLineups.length > 0) {
                                            let thisTeamLineup = teamLineups[0];

                                            if (thisTeamLineup) {
                                                teamLineupItem.dateSet = thisTeamLineup.dateSet;
                                                teamLineupItem.players = this.getLineupPlayers(team.id, thisTeamLineup.players);
                                            }
                                        }

                                        this.teamLineups.push(teamLineupItem);
                                    });
                                }
                            });
                    });
                }
            );
    }

    getLineupPlayers(teamId: number, playersSelected: number[]): TeamLineupPlayer[] {
        let teamLineupPlayers = [];

        this.playerService.getPlayersForTeam(teamId)
            .subscribe(playersForTeam => {
                playersSelected.forEach(playerId => {

                    let player = playersForTeam.filter(item => {
                        return item.id === playerId;
                    })[0];

                    if (player) {
                        let teamLineupPlayer = new TeamLineupPlayer();
                        teamLineupPlayer.name = player.fullName;
                        teamLineupPlayer.position = player.position;
                        teamLineupPlayers.push(teamLineupPlayer);
                    }
                });
            });

        return teamLineupPlayers;
    }
}
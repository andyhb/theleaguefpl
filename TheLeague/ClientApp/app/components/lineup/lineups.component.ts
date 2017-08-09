import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';

import { Team } from "../../models/team";
import { Lineup, TeamLineup } from "../../models/lineup";

import TeamService = require("../../services/team.service");
import LineupService = require("../../services/lineup.service");
import ResultService = require("../../services/result.service");

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

    constructor(private teamService: TeamService.TeamService, private lineupService: LineupService.LineupService, private resultService: ResultService.ResultService) { }

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
                                        teamLineupItem.team = team;

                                        let teamLineups = this.gameWeekLineups.teamLineups.filter(teamLineup => {
                                            return team.id === teamLineup.teamId;
                                        });

                                        if (teamLineups && teamLineups.length > 0) {
                                            teamLineupItem.teamLineup = teamLineups[0];
                                        }

                                        this.teamLineups.push(teamLineupItem);
                                    });
                                }
                            });
                    });
                }
            );
    }
}

export class TeamLineupItem {
    team: Team;
    teamLineup: TeamLineup;
}
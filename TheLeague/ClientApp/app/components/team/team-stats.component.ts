import { Component } from '@angular/core';
import { GoogleChartComponent } from '../charts/googlechart.component';

import { Team } from "../../models/team";
import { Stats, TeamPosition } from "../../models/stats";

import TeamService = require("../../services/team.service");
import StatsService = require("../../services/stats.service");

@Component({
    selector: 'team-stats',
    template: require('./team-stats.component.html')
})

export class TeamStatsComponent extends GoogleChartComponent {
    private options;
    private data;
    private chart;

    teams: Team[];
    stats: Stats;

    constructor(private teamService: TeamService.TeamService, private statsService: StatsService.StatsService) {
        super();
    }

    drawChart(): void {
        this.teamService.getTeams()
            .subscribe(
                teams => {
                    this.teams = teams;

                    this.statsService.getTeamPositions()
                        .subscribe(
                            stats => {
                                this.stats = stats;

                                let dataArray = [];
                                let titlesArray = ['Game Week'];

                                this.teams.forEach(team => {
                                    titlesArray.push(team.name);
                                });

                                dataArray.push(titlesArray);

                                this.stats.gameWeekPositions.forEach(gameWeekPosition => {
                                    let gameWeekArray = [gameWeekPosition.gameWeek];

                                    gameWeekPosition.teamPositions.forEach(teamPosition => {
                                        gameWeekArray.push(teamPosition.position);
                                    });

                                    dataArray.push(gameWeekArray);
                                });

                                this.data = this.createDataTable(dataArray);

                                this.options = {
                                    chartArea: { left: '2%', width: '80%', height: '80%' },
                                    hAxis: {
                                        minValue: 1,
                                        gridlines: {
                                            count: dataArray.length - 1
                                        }
                                    },
                                    vAxis: {
                                        direction: -1,
                                        minValue: 1,
                                        gridlines: {
                                            count: 9
                                        },
                                        viewWindow: {
                                            min: 1,
                                            max: 8
                                        }
                                    }
                                };

                                this.chart = this.createLineChart(document.getElementById('chart_teamRankings'));
                                this.chart.draw(this.data, this.options);
                            }
                        );
                }
            );
    }
}
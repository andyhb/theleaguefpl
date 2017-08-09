import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';

import { Team } from "../../models/team";
import { Result, TeamResult, FullTeamResult } from "../../models/result";

import TeamService = require("../../services/team.service");
import ResultService = require("../../services/result.service");

@Component({
    selector: 'results',
    template: require('./results-list.component.html'),
    styles: [require('./results-list.component.css')]
})

export class ResultsComponent implements OnInit {
    @Input() minified: boolean;

    start: number;
    end: number;
    startVisible: boolean = false;
    endVisible: boolean = false;
    resultWeeks: number[] = [];

    teams: Team[];
    result: Result;
    allResults: Result[];
    results: FullTeamResult[];
    currentGameWeek: number;

    constructor(private teamService: TeamService.TeamService, private resultService: ResultService.ResultService) { }

    ngOnInit(): void {
        this.teamService.getTeams()
            .subscribe(
            teams => this.teams = teams
            );

        this.resultService.getCurrentGameWeek()
        .subscribe(currentGameWeek =>
            {
                this.currentGameWeek = currentGameWeek;

                this.resultService.getAllResults()
                    .subscribe(
                    allResults => {
                        this.allResults = allResults;

                        if (this.allResults && this.allResults.length > 0) {
                            this.getFullTeamResults(this.allResults, false);

                            this.resultWeeks = this.allResults.map(result => {
                                return result.id;
                            });

                            if (this.resultWeeks.length > 0) {
                                this.start = this.resultWeeks[0];
                                this.end = this.resultWeeks[this.resultWeeks.length - 1];
                            }
                        }
                    }
                );
            }
        );
    }

    getFullTeamResults(allResults: Result[], changedBounds: boolean): void {
        this.results = [];
        let allButLatestResults = [];

        if (this.teams) {
            // go through each team and get totals EXCEPT FOR THE CURRENT GAMEWEEK!
            this.teams.forEach(team => {
                let fullTeamResult = new FullTeamResult;
                let teamTotal = 0;

                fullTeamResult.teamId = team.id;
                fullTeamResult.teamName = team.name;

                let cgw = this.currentGameWeek;
                if (changedBounds) {
                    cgw = Number(this.end);
                }

                // looking at each result
                allResults.forEach(result => {
                    // getting the result for current team for the week
                    let teamScoreForGw = result.teamResults.filter(teamResult => (teamResult.teamId === team.id))[0];

                    // getting the recent from the current gameweek
                    if (result.id === cgw) {
                        fullTeamResult.recentScore = teamScoreForGw.score;
                        if (teamScoreForGw.penalty > 0) {
                            fullTeamResult.penalty = teamScoreForGw.penalty;
                        }
                    } else {
                        // adding it to the total
                        teamTotal += teamScoreForGw.score;
                    }
                });

                fullTeamResult.totalScore = teamTotal;

                allButLatestResults.push(fullTeamResult);
            });

            // order based on total score
            allButLatestResults = allButLatestResults.sort((a, b) => (a.totalScore < b.totalScore ? 1 : a.totalScore > b.totalScore ? -1 : 0));

            //// add most recent...
            let allIncludingLatestResults = [];
            allButLatestResults.forEach(fullResult => {
                let newFullResult = new FullTeamResult;
                newFullResult.teamId = fullResult.teamId;
                newFullResult.totalScore = fullResult.totalScore;
                newFullResult.teamName = fullResult.teamName;
                newFullResult.recentScore = fullResult.recentScore;
                newFullResult.penalty = fullResult.penalty;
                allIncludingLatestResults.push(newFullResult);
            });
            
            allIncludingLatestResults.forEach(fullResult => {
                fullResult.totalScore += fullResult.recentScore;
            });

            // order most recent
            allIncludingLatestResults = allIncludingLatestResults.sort((a, b) => (a.totalScore < b.totalScore ? 1 : a.totalScore > b.totalScore ? -1 : 0));

            //// now calculate whether the team moved up or moved down to give position
            let position = 0;
            let previousTeamScore = allIncludingLatestResults[0].totalScore;

            allIncludingLatestResults.forEach(result => {
                result.position = position;

                let previousPosition = allButLatestResults.map(e => e.teamId).indexOf(result.teamId);
                result.previousPosition = previousPosition;
                result.pointsBehind = previousTeamScore - result.totalScore;

                position++;
                previousTeamScore = result.totalScore;
            });

            this.results = allIncludingLatestResults;
        }
    }

    getMovement(position: number, previousPosition: number): string {
        if (position === previousPosition) {
            return "";
        }

        let iconClass = "";

        if (position > previousPosition) {
            iconClass = "menu-up";
        }

        if (position < previousPosition) {
            iconClass = "menu-down";
        }

        return iconClass;
    }

    toggleBounds(type: string): void {
        if (type === "start") {
            this.startVisible = !this.startVisible;
        }

        if (type === "end") {
            this.endVisible = !this.endVisible;
        }
    }

    changeBounds(type: string, week: number) {
        if (type === "start") {
            this.start = week;
        }

        if (type === "end") {
            this.end = week;
        }

        if (this.end < this.start) {
            this.end = this.start;
        }

        this.resultService.getRangeOfResults(this.start, this.end)
            .subscribe(
            allResults => {
                this.allResults = allResults;

                if (this.allResults && this.allResults.length > 0) {
                    this.getFullTeamResults(this.allResults, true);
                }
            }
        );
    }

    getGameWeekInfo(): string {
        let gwString = "Gameweek ";

        if (this.start !== this.end) {
            return gwString + this.start + " - " + gwString + this.end;
        } else {
            return gwString + this.start;
        }
    }
}

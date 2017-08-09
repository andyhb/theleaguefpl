import { Component, OnInit } from '@angular/core';

import { Result, TeamResult, FullTeamResult } from "../../models/result";

import AuthService = require("../../services/auth.service");
import ResultService = require("../../services/result.service");
import LineupService = require("../../services/lineup.service");

@Component({
    selector: 'management',
    template: require('./management.component.html')
})

export class ManagementComponent implements OnInit {

    result: Result;
    currentGameWeek: number;

    constructor(private auth: AuthService.Auth,
        private resultService: ResultService.ResultService,
        private lineupService: LineupService.LineupService) { }

    ngOnInit(): void {
        this.resultService.getCurrentGameWeek()
            .subscribe(
                currentGameWeek => this.currentGameWeek = currentGameWeek
            );
    }

    updateAllResults(): void {
        this.resultService.updateAllResults(this.currentGameWeek)
            .subscribe(
                result => this.result = result
            );
    }
}
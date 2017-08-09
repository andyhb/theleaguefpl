import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';

import { Team } from "../../models/team";

import TeamService = require("../../services/team.service");
import ManagerService = require("../../services/manager.service");
import AuthService = require("../../services/auth.service");

@Component({
    selector: 'team-detail',
    template: require('./team-detail.component.html'),
    styles: [require('./team-detail.component.css')]
})

export class TeamDetailComponent implements OnInit, OnChanges {
    team: Team;
    @Input() teamId: number;
    @Input() teamSelection: boolean;
    init: boolean;

    constructor(private route: ActivatedRoute, private service: TeamService.TeamService, private managerService: ManagerService.ManagerService, private auth: AuthService.Auth) {}

    ngOnInit(): void {
        if (!this.teamId) {
            this.route
                .queryParams
                .subscribe(params => {
                    this.teamId = +params['teamId'];
                });
        }

        if (!this.teamId) {
            if (this.auth.teamId()) {
                this.teamId = this.auth.teamId();
            }
        }

        if (!this.teamSelection) {
            this.route
                .queryParams
                .subscribe(params => {
                    this.teamSelection = params['teamSelection'];
                });
        }

        this.getTeam(this.teamId);

        this.init = true;
    }

    ngOnChanges(changes): void {
        if (this.init) {
            if (changes.teamId) {
                this.getTeam(this.teamId);
            }
        }
    }

    getTeam(id: number) {
        this.service.getTeam(id)
            .subscribe(team => {
                this.team = this.getTeamDetails(team);
            });
    }

    getTeamDetails(team: Team): Team {
        let newTeam = new Team();

        newTeam.id = team.id;
        newTeam.name = team.name;
        newTeam.players = team.players;
        newTeam.manager = team.manager;

        this.managerService.getManager(team.manager)
            .subscribe(manager => {
                newTeam.managerName = manager.name;
            });

        return newTeam;
    }

    isTeamSelection(): boolean {
        if (this.teamSelection === undefined || !this.teamSelection) {
            return false;
        }

        return true;
    }
}
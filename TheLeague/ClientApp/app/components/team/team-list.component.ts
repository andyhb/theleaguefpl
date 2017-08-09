import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';

import { Team } from "../../models/team";
import { Manager } from "../../models/manager";
import TeamService = require("../../services/team.service");
import AuthService = require("../../services/auth.service");
import ManagerService = require("../../services/manager.service");

@Component({
    selector: 'teams',
    template: require('./team-list.component.html'),
    styles: [require('./team-list.component.css')]
})

export class TeamsComponent implements OnInit {
    teams: Team[] = [];
    selectedTeam: Team;

    constructor(private service: TeamService.TeamService, private auth: AuthService.Auth, private managerService: ManagerService.ManagerService) {}
    
    onSelect(team: Team): void {
        this.selectedTeam = team;
    }

    ngOnInit(): void {
        this.service.getTeams()
            .subscribe(
                teams => {
                    teams.forEach(tempTeam => {
                        let team = new Team();

                        team.id = tempTeam.id;
                        team.name = tempTeam.name;
                        team.players = tempTeam.players;
                        team.manager = tempTeam.manager;

                        this.managerService.getManager(tempTeam.manager)
                            .subscribe(manager => {
                                team.managerName = manager.name;
                            });

                        this.teams.push(team);
                    });
                }
        );
    }
}

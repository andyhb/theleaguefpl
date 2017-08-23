import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { tokenNotExpired } from 'angular2-jwt';
import { isBrowser } from 'angular2-universal';

let Auth0Lock: any = require('auth0-lock').default;

import { Team } from "../models/team";
import TeamService = require("./team.service");

@Injectable()
export class Auth {
    userProfile: any;
    currentTeam: Team;

    // Configure Auth0
    lock = new Auth0Lock('WEf07C3NEGE2BJrNVjF4ap1y6ExSiaIM', 'theleaguefpl.eu.auth0.com', {
        languageDictionary: {
            title: "The League Login"
        },
        theme: {
            primaryColor: "#4189C7"
        },
        allowSignUp: false
    });

    constructor(private router: Router, private teamService: TeamService.TeamService) {
        // set profile
        this.userProfile = JSON.parse(localStorage.getItem('profile'));

        // Add callback for lock `authenticated` event
        this.lock.on("authenticated",
            (authResult) => {
                localStorage.setItem('id_token', authResult.idToken);

                this.lock.getProfile(authResult.idToken,
                    (error, profile) => {
                        if (error) {
                            // Handle error
                            console.log(error);
                            return;
                        }

                        localStorage.setItem('profile', JSON.stringify(profile));
                        this.userProfile = profile;

                        var redirectUrl: string = localStorage.getItem('redirect_url');
                        if (redirectUrl != undefined) {
                            this.router.navigate([redirectUrl]);
                            localStorage.removeItem('redirect_url');
                        }

                        this.setCurrentTeam();
                    });
            });
    }

    public setCurrentTeam() {
        let managerId = this.managerId();

        if (typeof managerId === 'number') {
            this.teamService.getCurrentTeam(managerId)
                .subscribe(
                currentTeam => {
                    this.currentTeam = currentTeam;
                    localStorage.setItem('team_id', JSON.stringify(this.currentTeam.id));
                }
                );
        }
    }

    public login() {
        // Call the show method to display the widget.
        this.lock.show();
    }

    public authenticated() {
        // Check if there's an unexpired JWT
        // This searches for an item in localStorage with key == 'id_token'
        return tokenNotExpired();
    }

    public logout() {
        this.currentTeam = null;
        localStorage.removeItem('team_id');

        // Remove token from localStorage
        localStorage.removeItem('id_token');

        // remove profile
        localStorage.removeItem('profile');
        this.userProfile = undefined;
    }

    public isAdmin() {
        return this.authenticated() &&
            this.userProfile &&
            this.userProfile.app_metadata &&
            this.userProfile.app_metadata.roles &&
            this.userProfile.app_metadata.roles.indexOf('admin') > -1;
    }

    public managerId() {
        if (this.authenticated() &&
            this.userProfile &&
            this.userProfile.app_metadata &&
            this.userProfile.app_metadata.managerId) {
            return this.userProfile.app_metadata.managerId;
        }

        return null;
    }

    public teamId() {
        if (this.currentTeam) {
            return this.currentTeam.id;
        } else {
            var teamId = JSON.parse(localStorage.getItem('team_id'));

            if (teamId) {
                return teamId;
            }
        }

        return null;
    }
}
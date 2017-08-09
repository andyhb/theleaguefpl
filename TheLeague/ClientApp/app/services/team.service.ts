import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { Team } from "../models/team";

@Injectable()

export class TeamService {
    private teamUrl: string = '/api/Team/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get all teams
     */
    getTeams(): Observable<Team[]> {
        return (this.http.get(this.teamUrl + 'GetAllTeams')
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get team based on team id
     */
    getTeam(teamId: number): Observable<Team> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', teamId.toString());
        this.options.search = params;

        return (this.http.get(this.teamUrl + 'GetTeam', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get current season team based on manager id
     */
    getCurrentTeam(managerId: number): Observable<Team> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', managerId.toString());
        this.options.search = params;

        return (this.http.get(this.teamUrl + 'GetCurrentTeam', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get team that a player is on based on player id
     */
    findTeamForPlayer(playerId: number): Observable<Team> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('playerId', playerId.toString());
        this.options.search = params;

        return (this.http.get(this.teamUrl + 'FindTeamForPlayer', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
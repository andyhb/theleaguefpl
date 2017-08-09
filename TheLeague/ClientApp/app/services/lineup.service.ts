import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { Lineup, TeamLineup } from "../models/lineup";

@Injectable()

export class LineupService {
    private lineupUrl: string = '/api/Lineup/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get gameweek lineups based on gameweek id
     */
    getGameWeekLineup(currentGameWeek: number): Observable<Lineup> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', currentGameWeek.toString());
        this.options.search = params;

        return (this.http.get(this.lineupUrl + 'GetLineup', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get gameweek lineup for a team, based on gameweek id and team id
     */
    getGameWeekLineupForTeam(currentGameWeek: number, teamId: number): Observable<TeamLineup> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', currentGameWeek.toString());
        params.set('teamId', teamId.toString());
        this.options.search = params;

        return (this.http.get(this.lineupUrl + 'GetTeamLineup', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get all lineups
     */
    getAllLineups(): Observable<Lineup[]> {
        return (this.http.get(this.lineupUrl + 'GetAllLineups')
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Add a lineup
     */
    addLineup(gameWeek: number, teamId: number, players: number[]): Observable<TeamLineup> {
        let body = {
            id: gameWeek,
            teamId: teamId,
            players: players
        }
        let bodyJSON = JSON.stringify(body);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return (this.http.post(this.lineupUrl + 'AddTeamLineup', bodyJSON, options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
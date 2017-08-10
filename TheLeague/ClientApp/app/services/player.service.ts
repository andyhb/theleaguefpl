import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Subject } from 'rxjs/Subject';

import { Player } from "../models/player";

@Injectable()

export class PlayerService {
    private playerUrl: string = '/api/Player/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    searchParamChange: Subject<string> = new Subject<string>();
    searchParam: string;

    constructor(private http: Http) { }

    changeSearchParam(newSearchParam: string) {
        this.searchParam = newSearchParam;
        this.searchParamChange.next(this.searchParam);
    }

    /**
     * Get a player by id
     */
    getPlayer(playerId: number): Observable<Player> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('playerId', playerId.toString());
        this.options.search = params;

        return (this.http.get(this.playerUrl + 'GetPlayer', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get all players for a team based on team id
     */
    getPlayersForTeam(teamId: number): Observable<Player[]> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('teamId', teamId.toString());
        this.options.search = params;

        return (this.http.get(this.playerUrl + 'GetPlayersForTeam', this.options)
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get the players in the team of the week
     */
    getTeamOfTheWeek(): Observable<Player[]> {
        return (this.http.get(this.playerUrl + 'GetTeamOfTheWeek')
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Find the players that match the provided string
     */
    findPlayer(name: string): Observable<Player[]> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('name', name);
        this.options.search = params;

        return (this.http.get(this.playerUrl + 'FindPlayer', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
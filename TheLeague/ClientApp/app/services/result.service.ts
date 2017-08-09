import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { Result, TeamResult } from "../models/result";

@Injectable()

export class ResultService {
    private resultUrl: string = '/api/Result/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get current gameweek
     */
    getCurrentGameWeek(): Observable<number> {
        return (this.http.get(this.resultUrl + 'GetCurrentGameWeek')
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get gameweek results based on gameweek id
     */
    getGameWeekResult(currentGameWeek: number): Observable<Result> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', currentGameWeek.toString());
        this.options.search = params;

        return (this.http.get(this.resultUrl + 'GetResult', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get all results
     */
    getAllResults(): Observable<Result[]> {
        return (this.http.get(this.resultUrl + 'GetAllResults')
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get all results within supplied range
     */
    getRangeOfResults(start: number, end: number): Observable<Result[]> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('start', start.toString());
        params.set('end', end.toString());
        this.options.search = params;

        return (this.http.get(this.resultUrl + 'GetRangeOfResults', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Add a result
     */
    addResult(gameWeek: number, teamId: number, score: number, penalty: number): Observable<TeamResult> {
        let body = {
            id: gameWeek,
            teamId: teamId,
            score: score,
            penalty: penalty
        }
        let bodyJSON = JSON.stringify(body);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return (this.http.post(this.resultUrl + 'AddTeamResult', bodyJSON, options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Update all results
     */
    updateAllResults(gameWeek: number): Observable<Result> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', gameWeek.toString());
        this.options.search = params;

        return (this.http.get(this.resultUrl + 'UpdateAllGameWeekResultsFromSetLineups', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
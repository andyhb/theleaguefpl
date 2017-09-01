import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { Stats, TeamPosition } from "../models/stats";

@Injectable()

export class StatsService {
    private statsUrl: string = '/api/Stats/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get all team positions
     */
    getTeamPositions(): Observable<Stats> {
        return (this.http.get(this.statsUrl + 'GetTeamPositions')
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
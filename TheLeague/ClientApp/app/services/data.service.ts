import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { DataRequest } from "../models/datarequest";

@Injectable()

export class DataService {
    private dataUrl: string = '/api/Data/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get latest data request
     */
    getLatest(): Observable<DataRequest> {
        return (this.http.get(this.dataUrl + 'GetLatest')
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Send update data call and get new data request
     */
    update(): Observable<DataRequest> {
        return (this.http.get(this.dataUrl + 'Update')
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
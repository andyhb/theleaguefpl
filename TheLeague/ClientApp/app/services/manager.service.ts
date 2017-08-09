import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { Manager } from "../models/manager";

@Injectable()

export class ManagerService {
    private managerUrl: string = '/api/Manager/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    constructor(private http: Http) { }

    /**
     * Get all managers
     */
    getManagers(): Observable<Manager[]> {
        return (this.http.get(this.managerUrl + 'GetAllManagers')
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get manager based on manager id
     */
    getManager(managerId: number): Observable<Manager> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('id', managerId.toString());
        this.options.search = params;

        return (this.http.get(this.managerUrl + 'GetManager', this.options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
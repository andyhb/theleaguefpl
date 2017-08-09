import { Injectable } from '@angular/core';
import { Http, URLSearchParams, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Subject } from 'rxjs/Subject';

import { Player } from "../models/player";
import { Transfer, TeamTransfer } from "../models/transfer";

@Injectable()

export class TransferService {
    private transferUrl: string = '/api/Transfer/';
    private options = new RequestOptions({ headers: new Headers({ 'Content-Type': 'application/json' }) });

    initialPlayerToTranser: Player;

    playersChange: Subject<Player[]> = new Subject<Player[]>();
    playersAdded: Player[] = [];

    constructor(private http: Http) { }

    addPlayerToTransfer(player: Player) {
        if (this.playersAdded.indexOf(player) === -1) {
            this.playersAdded.push(player);
            this.playersChange.next(this.playersAdded);
        }
    }

    /**
     * Get all transfers
     */
    getAllTransfers(): Observable<Transfer[]> {
        return (this.http.get(this.transferUrl + 'GetAllTransfers')
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Get all transfers for a team based on team id
     */
    getTransfersForTeam(teamId: number): Observable<Transfer[]> {
        const params: URLSearchParams = new URLSearchParams();
        params.set('teamId', teamId.toString());
        this.options.search = params;

        return (this.http.get(this.transferUrl + 'GetTransfersForTeam', this.options)
                .map(response => response.json())
                .catch(err => { return this.handleError(err) })
        ) as any;
    }

    /**
     * Make the transfer
     */
    makeTransfer(teamTransfers: TeamTransfer[]): Observable<Transfer> {
        let body = {
            teamTransfers: teamTransfers
        }
        let bodyJSON = JSON.stringify(body);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return (this.http.post(this.transferUrl + 'MakeTransfer', bodyJSON, options)
            .map(response => response.json())
            .catch(err => { return this.handleError(err) })
        ) as any;
    }

    private handleError(err) {
        const errorMessage: string = err.message ? err.message : err.toString();
        return Observable.throw(errorMessage);
    }
}
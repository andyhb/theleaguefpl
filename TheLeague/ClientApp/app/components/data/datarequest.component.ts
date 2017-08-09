import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { DataRequest } from "../../models/datarequest";

import { Auth } from '../../services/auth.service';
import DataService = require("../../services/data.service");

@Component({
    selector: 'datarequest',
    template: require('./datarequest.component.html'),
    styles: [require('./datarequest.component.css')]
})
export class DataRequestComponent implements OnInit {
    dataRequest: DataRequest;

    constructor(private service: DataService.DataService, private auth: Auth) {}

    ngOnInit(): void {
        this.service.getLatest()
            .subscribe(
                dataRequest => this.dataRequest = dataRequest
        );
    }

    update(): void {
        this.service.update()
            .subscribe(
                dataRequest => this.dataRequest = dataRequest);
    }

    updateAllowed(): boolean {
        let hourInMs = 60 * 60 * 1000;
        let currentDate = new Date().valueOf();
        let lastRequestDate = new Date(this.dataRequest.requestDate).valueOf();

        if ((currentDate - lastRequestDate) < hourInMs) {
            return false;
        }

        return true;
    }
}
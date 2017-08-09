import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';

import { Transfer, TeamTransfer, PlayerTransferred, PlayerTransferredItem, FullTransferItem } from "../../models/transfer";
import TransferService = require("../../services/transfer.service");
import AuthService = require("../../services/auth.service");

@Component({
    selector: 'transfer-list',
    template: require('./transfer-list.component.html'),
    styles: [require('./transfer-list.component.css')]
})

export class TransferListComponent implements OnInit {
    transfers: Transfer[];
    fullTransfers: FullTransferItem[];

    constructor(private transferService: TransferService.TransferService, private auth: AuthService.Auth) { }

    ngOnInit(): void {
        this.transferService.getAllTransfers()
            .subscribe(
                transfers => {
                    this.transfers = transfers;


                }
            );
    }
}

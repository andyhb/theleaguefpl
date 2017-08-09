import { Component } from '@angular/core';
import Authservice = require("../../services/auth.service");

@Component({
    selector: 'app',
    template: require('./app.component.html'),
    styles: [require('./app.component.css')]
})

export class AppComponent {
    constructor(private auth: Authservice.Auth) {}
}

import { Component } from '@angular/core';
import Authservice = require("../../services/auth.service");

@Component({
    selector: 'nav-menu',
    template: require('./navmenu.component.html'),
    styles: [require('./navmenu.component.css')]
})
export class NavMenuComponent {
    constructor(private auth: Authservice.Auth) {}
}

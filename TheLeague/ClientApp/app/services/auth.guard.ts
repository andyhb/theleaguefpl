import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CanActivate } from '@angular/router';
import { Auth } from './auth.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private auth: Auth, private router: Router) {}

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.auth.authenticated()) {
            var checkRoles = false;

            let roles = next.data["roles"] as Array<string>;
            if (roles != null && roles.indexOf("admin") > -1) {
                checkRoles = true;
            }

            if (checkRoles || this.auth.isAdmin()) {
                return true;
            } else {
                if (this.auth.teamId() == next.queryParams["teamId"]) {
                    return true;
                }

                this.router.navigate(['unauthorized']);
                return false;
            }
        } else {
            // Save URL to redirect to after login and fetching profile to get roles
            localStorage.setItem('redirect_url', state.url);
            this.auth.login();
            this.router.navigate(['']);
            return false;
        }
    }
}
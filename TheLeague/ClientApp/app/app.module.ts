import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule }   from '@angular/forms';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { DataRequestComponent } from './components/data/datarequest.component';
import { ManagementComponent } from './components/management/management.component';

import { TeamsComponent } from './components/team/team-list.component';
import { TeamDetailComponent } from './components/team/team-detail.component';

import { PlayersComponent } from './components/player/player-list.component';

import { TransferComponent } from './components/transfer/transfer.component';
import { TransferListComponent } from './components/transfer/transfer-list.component';

import { ResultsComponent } from './components/result/results-list.component';

import { LineupsComponent } from './components/lineup/lineups.component';

import { GetPositionStringPipe } from "./pipes/get-position-string.pipe";
import { FormatDatePipe } from "./pipes/format-date.pipe";
import { GetFixturesStringPipe } from "./pipes/get-fixtures-string.pipe";

import { TeamService } from "./services/team.service";
import { PlayerService } from "./services/player.service";
import { DataService } from "./services/data.service";
import { ResultService } from "./services/result.service";
import { LineupService } from "./services/lineup.service";
import { TransferService } from "./services/transfer.service";
import { ManagerService } from "./services/manager.service";

import { Auth } from "./services/auth.service";
import { AuthGuard } from "./services/auth.guard";
import { AUTH_PROVIDERS } from 'angular2-jwt';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        DataRequestComponent,
        ManagementComponent,
        TeamsComponent,
        TeamDetailComponent,
        PlayersComponent,
        TransferComponent,
        TransferListComponent,
        ResultsComponent,
        LineupsComponent,
        GetPositionStringPipe,
        FormatDatePipe,
        GetFixturesStringPipe
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'team-list', component: TeamsComponent },
            { path: 'results-list', component: ResultsComponent },
            { path: 'transfer-list', component: TransferListComponent },
            { path: 'team-detail', component: TeamDetailComponent, canActivate: [AuthGuard]/*, data: { roles: ['admin'] } */ },
            { path: 'management', component: ManagementComponent, canActivate: [AuthGuard], data: { roles: ['admin'] } },
            { path: 'transfer', component: TransferComponent, canActivate: [AuthGuard], data: { roles: ['admin'] } },
            { path: '**', redirectTo: 'home' }
        ]),
        FormsModule
    ],
    providers: [
        TeamService,
        PlayerService,
        DataService,
        ResultService,
        LineupService,
        TransferService,
        ManagerService,
        AUTH_PROVIDERS,
        Auth,
        AuthGuard
    ]
})

export class AppModule {
}

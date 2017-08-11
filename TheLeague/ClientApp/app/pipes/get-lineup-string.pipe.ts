import { Pipe, PipeTransform } from '@angular/core';
import { TeamLineupPlayer } from "../models/lineup";

@Pipe({ name: 'getLineupString' })
export class GetLineupStringPipe implements PipeTransform {
    transform(players: TeamLineupPlayer[]): string {
        let fullString = "";

        console.log(players);

        if (players) {
            
            let gkString = "<strong>GK:</strong> ";
            let defString = "<strong>DEF:</strong> ";
            let midString = "<strong>MID:</strong> ";
            let forString = "<strong>FOR:</strong> ";

            let gk = players.filter(player => (player.position === 1))[0];

            gkString += gk.name;

            let defs = players.filter(player => {
                    return player.position === 2;
                })
                .map(player => {
                    return player.name;
                });

            defString += defs.join(', ');

            let mids = players.filter(player => {
                    return player.position === 3;
                })
                .map(player => {
                    return player.name;
                });

            midString += mids.join(', ');

            let fors = players.filter(player => {
                    return player.position === 4;
                })
                .map(player => {
                    return player.name;
                });

            forString += fors.join(', ');

            fullString += gkString + "<br />" + defString + "<br />" + midString + "<br />" + forString;
        }

        return fullString;
    }
}
import { Pipe, PipeTransform } from '@angular/core';
import { TeamLineupPlayer } from "../models/lineup";

@Pipe({ name: 'getLineupString' })
export class GetLineupStringPipe implements PipeTransform {
    transform(players: TeamLineupPlayer[]): string {
        let fullString = "";

        if (players) {
            
            let gkString = "<strong>GK:</strong> ";
            let defString = "<strong>DEF:</strong> ";
            let midString = "<strong>MID:</strong> ";
            let forString = "<strong>FOR:</strong> ";

            let gk = players.filter(player => (player.position === 1))[0];

            if (gk) {
                gkString += gk.name;
            } else {
                gkString += "N/A";
            }

            let defs = players.filter(player => {
                    return player.position === 2;
                })
                .map(player => {
                    if (player) {
                        return player.name;
                    } else {
                        return "N/A";
                    }
                });

            defString += defs.join(', ');

            let mids = players.filter(player => {
                    return player.position === 3;
                })
                .map(player => {
                    if (player) {
                        return player.name;
                    } else {
                        return "N/A";
                    }
                });

            midString += mids.join(', ');

            let fors = players.filter(player => {
                    return player.position === 4;
                })
                .map(player => {
                    if (player) {
                        return player.name;
                    } else {
                        return "N/A";
                    }
                });

            forString += fors.join(', ');

            fullString += gkString + "<br />" + defString + "<br />" + midString + "<br />" + forString;
        }

        return fullString;
    }
}
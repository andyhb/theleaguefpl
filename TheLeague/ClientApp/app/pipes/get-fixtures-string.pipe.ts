import { Pipe, PipeTransform } from '@angular/core';
import { EventFixture } from "../models/player";

@Pipe({ name: 'getFixturesString' })
export class GetFixturesStringPipe implements PipeTransform {
    transform(fixtures: EventFixture[], playerTeam): string {
        let fixtureString = [];

        if (!fixtures || fixtures.length === 0) {
            return "";
        }

        fixtures.forEach(fixture => {
            if (fixture.home) {
                fixtureString.push("<strong>" + playerTeam + "</strong> v " + fixture.opponent);
            } else {
                fixtureString.push(fixture.opponent + " v <strong>" + playerTeam + "</strong>");
            }
        });

        return fixtureString.join(", ");
    }
}
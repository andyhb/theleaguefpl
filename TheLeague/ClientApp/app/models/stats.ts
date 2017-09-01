export class Stats {
    gameWeekPositions: GameWeekPosition[];
}

export class GameWeekPosition {
    gameWeek: number;
    teamPositions: TeamPosition[];
}

export class TeamPosition {
    position: number;
    teamId: number;
}
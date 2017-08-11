export class Lineup {
    id: LineupId;
    teamLineups: TeamLineup[];
}

export class LineupId {
    gameWeek: number;
    season: number;
}

export class TeamLineup {
    teamId: number;
    players: number[];
    dateSet: string;
}

export class TeamLineupPlayer {
    position: number;
    name: string;
}

export class TeamLineupItem {
    teamName: string;
    dateSet: string;
    players: TeamLineupPlayer[];
}
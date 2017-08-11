export class Lineup {
    id: number;
    teamLineups: TeamLineup[];
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
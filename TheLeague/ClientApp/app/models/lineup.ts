export class Lineup {
    id: number;
    teamLineups: TeamLineup[];
}

export class TeamLineup {
    teamId: number;
    players: number[];
    dateSet: string;
}
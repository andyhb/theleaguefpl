export class Result {
    id: number;
    teamResults: TeamResult[];
}

export class TeamResult {
    teamId: number;
    score: number;
    penalty: number;
}

export class FullTeamResult {
    teamId: number;
    teamName: string;
    totalScore: number;
    recentScore: number;
    position: number;
    previousPosition: number;
    penalty: number;
    pointsBehind: number;
}
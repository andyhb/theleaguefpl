export class Player {
    id: number;
    fullName: string;
    totalPoints: number;
    recentPoints: number;
    position: number;
    teamName: string;
    currentFixtures: EventFixture[];
    nextFixtures: EventFixture[];
    webName: string;
    chanceOfPlayingPercentage: number;
    news: string;
}

export class EventFixture {
    opponent: string;
    home: boolean;
}
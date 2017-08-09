export class Transfer {
    id: number;
    transferDate: string;
    teamTransfers: TeamTransfer[];
}

export class TeamTransfer {
    transferredTo: number;
    playersTransferred: PlayerTransferred[];
}

export class PlayerTransferred {
    transferredFrom: number;
    playerId: number;
}

export class PlayerTransferredItem {
    transferredFromTeamName: string;
    playerName: string;
}

export class FullTransferItem {
    transferDate: string;
    transferredToTeamName: string;
    playersTransferred: PlayerTransferredItem[];
}
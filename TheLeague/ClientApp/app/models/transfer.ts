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
    transferredToTeamName: string;
    playerName: string;
}

export class FullTransferItem {
    transferDate: string;
    playersTransferred: PlayerTransferredItem[];
}
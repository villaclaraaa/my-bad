export interface Player {
    accountId: number;
    playerInfo: PlayerInfo;
}

export interface PlayerInfo {
    accountId: number;
    personaName: string;
    avatarMediumUrl: string;
}
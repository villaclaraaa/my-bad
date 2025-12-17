import { BaseApiResponse } from "./baseApiResponse";

export interface Player extends BaseApiResponse {
    accountId: number;
    playerInfo: PlayerInfo;
}

export interface PlayerInfo {
    accountId: number;
    personaName: string;
    avatarMediumUrl: string;
}
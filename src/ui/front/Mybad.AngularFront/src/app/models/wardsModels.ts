import { BaseApiResponse } from "./baseApiResponse";

// Interfaces related to Api response models.

export interface WardsMapApiResponse extends BaseApiResponse {
    observerWards: WardSimpleMap[];
    sentryWards: WardSimpleMap[];
    accountId: number;
}

export interface WardsEffApiResponse extends BaseApiResponse {
    observerWards: WardSimpleEfficiency[];
    includedMatches: ParsedMatchWardEfficiency[];
    totalWardsPlaced: number;
    wardsDistinctPositions: number;
    accountId: number;
}

export interface WardSimpleMap {
    x: number;
    y: number;
    amount: number;
    efficiency?: number;
}

export interface WardSimpleEfficiency {
    x: number;
    y: number;
    amount: number;
    averageTimeLived: number;
    efficiencyScore: number;
    isRadiantSide: boolean;
}

export interface ParsedMatchWardEfficiency {
    matchId: number;
    accountId: number;
    heroName: string;
    isRadiantPlayer: boolean;
    isWonMatch: boolean;
    playedAtDateUtc: Date;
}


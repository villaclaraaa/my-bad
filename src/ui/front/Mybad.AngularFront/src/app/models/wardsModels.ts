export interface WardSimpleMap {
    x: number;
    y: number;
    amount: number;
    efficiency?: number;
}

export interface WardsMapApiResponse {
    observerWards: WardSimpleMap[];
    sentryWards: WardSimpleMap[];
    accountId: number;
    id: number;
    errors: string[];
}

export interface WardSimpleEfficiency {
    x: number;
    y: number;
    amount: number;
    averageTimeLived: number;
    efficiencyScore: number;
    isRadiantSide: boolean;
}

export interface WardsEffApiResponse {
    observerWards: WardSimpleEfficiency[];
    includedMatches: ParsedMatchWardEfficiency[];
    totalWardsPlaced: number;
    wardsDistinctPositions: number;
    accountId: number;
    id: number;
    errors: string[];
}

export interface ParsedMatchWardEfficiency {
    matchId: number;
    accountId: number;
    heroName: string;
    isRadiantPlayer: boolean;
    isWonMatch: boolean;
    playedAtDateUtc: Date;
}


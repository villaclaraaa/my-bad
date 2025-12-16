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
}

export interface WardsEffApiResponse {
    observerWards: WardSimpleEfficiency[];
    includedMatches: number[];
    totalWardsPlaced: number;
    wardsDistinctPositions: number;
    accountId: number;
    id: number;
    errors: string[];
}


export interface WardSimple {
    x: number;
    y: number;
    amount: number 
}

export interface WardsMapApiResponse {
    observerWards: WardSimple[];
    sentryWards: WardSimple[];
    accountId: number;
    id: number;
    errors: string[];
}

export interface WardEfficiency {
    x: number;
    y: number;
    amount: number;
    averageTimeLived: number;
    efficiencyScore: number;
}

export interface WardsEffApiResponse {
    observerWards: WardEfficiency[];
    includedMatches: number[];
    totalWardsPlaced: number;
    wardsDistinctPositions: number;
    accountId: number;
    id: number;
    errors: string[];
}


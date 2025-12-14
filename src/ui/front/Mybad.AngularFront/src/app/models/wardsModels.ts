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
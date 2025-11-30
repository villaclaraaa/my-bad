export interface WardSimple {
    X: number;
    Y: number;
    Amount: number 
}

export interface WardsMapApiResponse {
    ObserverWards: WardSimple[];
    SentryWards: WardSimple[];
    AccountId: number;
}
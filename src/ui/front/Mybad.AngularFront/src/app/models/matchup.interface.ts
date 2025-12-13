export interface HeroMatchup {
    heroId: number;
    heroName: string;
    rating: number;
}

export interface HeroMatchupResponse {
    matchup: HeroMatchup[];
}

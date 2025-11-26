namespace Mybad.Core.DomainModels
{
    public class HeroMatchupModel
    {
        public int HeroId { get; set; }
        public int OtherHeroId { get; set; }
        public int Wins {  get; set; }
        public int GamesPlayed { get; set; }
    }
}

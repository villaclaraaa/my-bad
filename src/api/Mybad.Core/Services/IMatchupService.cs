using Mybad.Core.Providers.CoreHeroMatchupProvider;

namespace Mybad.Core.Services
{
    public interface IMatchupService
    {
        Task UpdateMatchups(
            Dictionary<(int HeroId, int OtherHeroId), GamesResultsStat> heroStats,
            bool isEnemyMatchup);

        Task<List<HeroMatchupModel>> CalcutaleBestMatchupVersusEnemies(List<int> ids);
        Task<List<HeroMatchupModel>> CalcutaleBestMatchupWithAllies(List<int> ids);
        Task<List<HeroMatchupModel>> CalcutaleBestMatchupCombined(List<int> enemyIds, List<int> allyIds);
    }
}

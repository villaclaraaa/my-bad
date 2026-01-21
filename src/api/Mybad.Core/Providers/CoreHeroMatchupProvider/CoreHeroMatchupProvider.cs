using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Services;

namespace Mybad.Core.Providers.CoreHeroMatchupProvider
{
    public class CoreHeroMatchupProvider : IInfoProvider<HeroMatchupRequest, HeroMatchupResponse>
    {
        private readonly IMatchupService _matchupService;
        public CoreHeroMatchupProvider(IMatchupService matchupService)
        {
            _matchupService = matchupService;
        }
        public async Task<HeroMatchupResponse> GetInfoAsync(HeroMatchupRequest request)
        {
            if (request.AllyIds != null && request.EnemyIds != null)
            {
                return await FindBestHeroVsEnemyWithAlly(request.EnemyIds, request.AllyIds, request.HeroesInPool);
            }
            else if (request.EnemyIds != null)
            {
                return await FindBestHeroVsEnemy(request.EnemyIds, request.HeroesInPool);
            }
            else if (request.AllyIds != null)
            {
                return await FindBestHeroWithAlly(request.AllyIds, request.HeroesInPool);
            }
            else
            {
                throw new ArgumentException("No ids to find matchup");
            }
        }
        private async Task<HeroMatchupResponse> FindBestHeroVsEnemyWithAlly(List<int> enemyIds, List<int> allyIds, List<int>? heroesInPool)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupCombined(enemyIds, allyIds, heroesInPool);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroVsEnemy(List<int> enemyIds, List<int>? heroesInPool)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupVersusEnemies(enemyIds, heroesInPool);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroWithAlly(List<int> allyIds, List<int>? heroesInPool)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupWithAllies(allyIds, heroesInPool);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }
    }

}

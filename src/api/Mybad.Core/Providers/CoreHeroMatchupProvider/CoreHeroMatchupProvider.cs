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
                return await FindBestHeroVsEnemyWithAlly(request.EnemyIds, request.AllyIds);
            }
            else if (request.EnemyIds != null)
            {
                return await FindBestHeroVsEnemy(request.EnemyIds);
            }
            else if (request.AllyIds != null)
            {
                return await FindBestHeroWithAlly(request.AllyIds);
            }
            else
            {
                throw new ArgumentException("No ids to find matchup");
            }
        }
        private async Task<HeroMatchupResponse> FindBestHeroVsEnemyWithAlly(List<int> enemyIds, List<int> allyIds)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupCombined(enemyIds, allyIds);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroVsEnemy(List<int> enemyIds)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupVersusEnemies(enemyIds);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroWithAlly(List<int> allyIds)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupWithAllies(allyIds);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }
    }

}

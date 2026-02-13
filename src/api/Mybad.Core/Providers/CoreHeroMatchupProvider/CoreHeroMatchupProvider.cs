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
        //TODO
        //REWRITE THE FUNCTIONS TO GET ONLY REQUEST NOT THAT MANY PARAMETERS
        public async Task<HeroMatchupResponse> GetInfoAsync(HeroMatchupRequest request)
        {
            if (request.AllyIds != null && request.EnemyIds != null)
            {
                return await FindBestHeroVsEnemyWithAlly(request.EnemyIds, request.AllyIds, request.HeroesInPool, request.patchId);
            }
            else if (request.EnemyIds != null)
            {
                return await FindBestHeroVsEnemy(request.EnemyIds, request.HeroesInPool, request.patchId);
            }
            else if (request.AllyIds != null)
            {
                return await FindBestHeroWithAlly(request.AllyIds, request.HeroesInPool, request.patchId);
            }
            else
            {
                throw new ArgumentException("No ids to find matchup");
            }
        }
        private async Task<HeroMatchupResponse> FindBestHeroVsEnemyWithAlly(List<int> enemyIds, List<int> allyIds, List<int>? heroesInPool, int? patchId)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupCombined(enemyIds, allyIds, heroesInPool, patchId);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroVsEnemy(List<int> enemyIds, List<int>? heroesInPool, int? patchId)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupVersusEnemies(enemyIds, heroesInPool, patchId);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }

        private async Task<HeroMatchupResponse> FindBestHeroWithAlly(List<int> allyIds, List<int>? heroesInPool, int? patchId)
        {
            var matchup = await _matchupService.CalcutaleBestMatchupWithAllies(allyIds, heroesInPool, patchId);

            var converter = new HeroMatchupConverter();

            return converter.ConvertHeroMatchup(matchup);
        }
    }

}

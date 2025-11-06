using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;

namespace Mybad.Services.OpenDota.ApiReponseReaders
{
    internal class HeroMatchupsReader
    {
        public HeroMatchupResponse ConvertHeroMatchups(List<HeroMatchupModel> heroMatchupModels)
        {
            var response = new HeroMatchupResponse();
            response.HeroMatchups = new List<HeroMatchup>();

            foreach (var item in heroMatchupModels)
            {
                string heroName = ((HeroesEnum)item.HeroId).ToString();
                double winrate = Math.Round(((double)item.Wins / (double)item.GamesPlayed) * 100d, 2);

                response.HeroMatchups.Add(new HeroMatchup() { HeroName = heroName, WinrateAgainst = winrate });
            }

            return response;
        }

    }
}

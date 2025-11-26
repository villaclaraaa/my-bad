using Mybad.Core.Enums;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Providers.CoreHeroMatchupProvider
{
    internal class HeroMatchupConverter
    {
        public HeroMatchupResponse ConvertHeroMatchup(List<HeroMatchupModel> matchup)
        {
            var response = new HeroMatchupResponse();
            response.Matchup = new List<HeroMatchup>();

            foreach (var item in matchup)
            {
                string heroName = ((HeroesEnum)item.HeroId).ToString();

                response.Matchup.Add(new HeroMatchup() { HeroId = item.HeroId, HeroName = heroName, Rating = item.Rating });
            }

            return response;
        }
    }
}
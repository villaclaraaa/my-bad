using Mybad.Core.Enums;
using Mybad.Core.Models;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Converters
{
    internal class HeroMatchupConverter
    {
        public HeroMatchupResponse ConvertHeroMatchups(List<HeroMatchupModel> bestCalculated,
            List<HeroMatchupModel> bestVersus, List<HeroMatchupModel> bestWith)
        {
            var response = new HeroMatchupResponse();
            response.BestVersus = new List<HeroMatchup>();
            response.BestWith = new List<HeroMatchup>();
            response.BestCalculated = new List<HeroMatchup>();


            foreach (var item in bestCalculated)
            {
                string heroName = ((HeroesEnum)item.HeroId).ToString();

                response.BestCalculated.Add(new HeroMatchup() { HeroId = item.HeroId, HeroName = heroName, Rating = item.Rating });
            }
            foreach (var item in bestVersus)
            {
                string heroName = ((HeroesEnum)item.HeroId).ToString();

                response.BestVersus.Add(new HeroMatchup() { HeroId = item.HeroId, HeroName = heroName, Rating = item.Rating });
            }
            foreach (var item in bestWith)
            {
                string heroName = ((HeroesEnum)item.HeroId).ToString();

                response.BestWith.Add(new HeroMatchup() { HeroId = item.HeroId, HeroName = heroName, Rating = item.Rating });
            }


            return response;
        }
    }
}

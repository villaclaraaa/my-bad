using Mybad.Core.Services;
using Mybad.Services.OpenDota.ApiResponseModels;

namespace CoreHeroMatchupsTests
{
    public class HeroMatchupTests
    {
        [Fact]
        public async Task GetWinrateVsAntiMageAxeAsync()
        {
            //Arrange
            List<int> heroesId = new List<int> { 1, 2 };

            //Act
            var bestHeroes = await CoreHeroMatchupProvider.CalculateBestHeroesAgainstEnemyPick(heroesId);
            List<string> expect = new List<string> { "NagaSiren", "Enigma", "BountyHunter", "EarthSpirit", "Bane" };
            List<string> res = new List<string>();
            foreach (var hero in bestHeroes)
            {
                res.Add(Enum.GetName(typeof(HeroesEnum), hero));
            }

            //Assert
            Assert.Equal(expect, res);
        }
    }
}
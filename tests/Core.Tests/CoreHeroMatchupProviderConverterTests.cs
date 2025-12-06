using Mybad.Core.Providers.CoreHeroMatchupProvider;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;
using HeroMatchupModel = Mybad.Core.Providers.CoreHeroMatchupProvider.HeroMatchupModel;

namespace Core.Tests;

public class CoreHeroMatchupProviderConverterTests
{
	[Fact]
	public void ConvertHeroMatchup_Returns_HeroMatchupResponse()
	{
		// Arrange 
		List<HeroMatchupModel> matchup = new()
		{
			new HeroMatchupModel { HeroId = 1, Rating = 10 },
			new HeroMatchupModel { HeroId = 2, Rating = 20 },
			new HeroMatchupModel { HeroId = 3, Rating = 1 },
			new HeroMatchupModel { HeroId = 4, Rating = 90 },
			new HeroMatchupModel { HeroId = 5, Rating = 10 },
		};
		HeroMatchupResponse expected = new HeroMatchupResponse
		{
			Matchup = new List<HeroMatchup>()
			{
				new HeroMatchup { HeroId = 1, HeroName = "AntiMage", Rating = 10 },
				new HeroMatchup { HeroId = 2, HeroName = "Axe", Rating = 20 },
				new HeroMatchup { HeroId = 3, HeroName = "Bane", Rating = 1 },
				new HeroMatchup { HeroId = 4, HeroName = "Bloodseeker", Rating = 90 },
				new HeroMatchup { HeroId = 5, HeroName = "CrystalMaiden", Rating = 10 },
			}
		};
		var converter = new HeroMatchupConverter();

		// Act 
		HeroMatchupResponse result = converter.ConvertHeroMatchup(matchup);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected.Matchup.Count, result.Matchup.Count);
		foreach (var expectedItem in expected.Matchup)
		{
			Assert.Contains(result.Matchup, actual =>
				actual.HeroId == expectedItem.HeroId &&
				actual.HeroName == expectedItem.HeroName &&
				actual.Rating == expectedItem.Rating);
		}
	}
}

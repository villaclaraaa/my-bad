using Mybad.Core.DomainModels;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Text.Json;

namespace OpenDotaService.Tests;

public class ApiResponseConverterTests
{
	[Fact]
	public async Task ConvertWardMapPlacementResponse_Returns_BothListsWards()
	{
		// Arrange 
		var apiResponseFilePath = "Data/wardmap.txt";
		var json = File.ReadAllText(apiResponseFilePath);
		var obsCount = 22;
		var distinctObsCount = 19;
		var senCount = 50;
		var distinctSenCount = 40;

		// Act 
		var data = JsonSerializer.Deserialize<WardPlacementMap>(json);
		Assert.NotNull(data);

		var reader = new WardsConverter();
		var (obses, sens) = reader.ConvertToWardList(data!);
		int oCount = 0, sCount = 0;
		foreach (var kvp in obses)
		{
			oCount += kvp.Amount;
		}

		foreach (var kvp in sens)
		{
			sCount += kvp.Amount;
		}

		// Assert
		Assert.Equal(distinctObsCount, obses.Count);
		Assert.Equal(distinctSenCount, sens.Count);
		Assert.Equal(obsCount, oCount);
		Assert.Equal(senCount, sCount);
	}

	[Fact]
	public void GetWardsForPlayerPerMatch_ReturnsWarsLogMatchResponse()
	{
		// Arrange
		var apiResponseFilePath = "Data/matchWards.txt";
		var json = File.ReadAllText(apiResponseFilePath);
		var accountId = 136996088;
		var obsCount = 3;
		var senCount = 12;
		var firstObs = new WardLog { Amount = 1, TimeLived = 360, X = 174, Y = 102, WasDestroyed = false };
		var secondObs = new WardLog { Amount = 1, TimeLived = 360, X = 132, Y = 183, WasDestroyed = false };
		var thirdObs = new WardLog { Amount = 1, TimeLived = 360, X = 163, Y = 156, WasDestroyed = false };
		var firstSentry = new WardLog { Amount = 1, X = 162, Y = 95, WasDestroyed = true, TimeLived = 32 };
		var secondSentry = new WardLog { Amount = 1, X = 162, Y = 95, WasDestroyed = true, TimeLived = 272 };
		var thirdSentry = new WardLog { Amount = 1, X = 110, Y = 161, WasDestroyed = false, TimeLived = 420 };

		// Act 
		var data = JsonSerializer.Deserialize<MatchWardLogInfo>(json);
		var reader = new WardsConverter();
		var (obses, sens) = reader.ConvertWardsLogMatch(data!, accountId);

		// Assert
		Assert.Equal(obsCount, obses.Count);
		Assert.Equal(senCount, sens.Count);
		Assert.Equivalent(firstObs, obses[0]);
		Assert.Equivalent(secondObs, obses[1]);
		Assert.Equivalent(thirdObs, obses[2]);
		Assert.Equivalent(firstSentry, sens[0]);
		Assert.Equivalent(secondSentry, sens[1]);
		Assert.Equivalent(thirdSentry, sens[2]);
	}

	[Fact]
	public async Task ConvertWardsToWardModel_Returns_ListWardModels()
	{
		// Arrange
		var apiResponseFilePath = "Data/8557298604_full.txt";
		var json = File.ReadAllText(apiResponseFilePath);
		var accountId = 136996088;
		var obsesCount = 7;
		var obsesList = new List<WardModel>() {
			new WardModel { AccountId = accountId, PosX = 90, PosY = 146, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 259, WasDestroyed = true },
			new WardModel { AccountId = accountId, PosX = 122, PosY = 122, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 360, WasDestroyed = false },
			new WardModel { AccountId = accountId, PosX = 113, PosY = 151, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 360, WasDestroyed = false },
			new WardModel { AccountId = accountId, PosX = 120, PosY = 65, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 360, WasDestroyed = false },
			new WardModel { AccountId = accountId, PosX = 119, PosY = 65, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 360, WasDestroyed = false },
			new WardModel { AccountId = accountId, PosX = 99, PosY = 88, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 98, WasDestroyed = true },
			new WardModel { AccountId = accountId, PosX = 94, PosY = 120, MatchId = 8557298604, Amount = 1, TimeLivedSeconds = 360, WasDestroyed = false }
		};

		// Act
		var data = JsonSerializer.Deserialize<MatchWardLogInfo>(json);
		Assert.NotNull(data);

		var converter = new WardsConverter();
		List<WardModel> result = converter.ConvertWardsToWardModel(data, accountId, 8557298604, isObs: true);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(obsesCount, result.Count);
		Assert.Contains(obsesList[0], result);
		Assert.Contains(obsesList[1], result);
		Assert.Contains(obsesList[2], result);
		Assert.Contains(obsesList[3], result);
		Assert.Contains(obsesList[4], result);
		Assert.Contains(obsesList[5], result);
		Assert.Contains(obsesList[6], result);
	}
}

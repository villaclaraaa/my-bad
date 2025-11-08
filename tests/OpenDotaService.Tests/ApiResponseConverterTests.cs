using System.Text.Json;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;

namespace OpenDotaService.Tests;

public class ApiResponseConverterTests
{
	[Fact]
	public void GetWardsPlacementMap_ReturnsWardsPlacementResponse()
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
		var reader = new WardsPlacementMapReader();
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
		var firstObs = new WardLog
		{
			Amount = 1,
			TimeLived = 360,
			X = 174,
			Y = 102,
			WasDestroyed = false
		};
		var secondObs = new WardLog
		{
			Amount = 1,
			TimeLived = 360,
			X = 132,
			Y = 183,
			WasDestroyed = false
		};
		var thirdObs = new WardLog
		{
			Amount = 1,
			TimeLived = 360,
			X = 163,
			Y = 156,
			WasDestroyed = false
		};
		var firstSentry = new WardLog
		{
			Amount = 1,
			X = 162,
			Y = 95,
			WasDestroyed = true,
			TimeLived = 32,
		};
		var secondSentry = new WardLog
		{
			Amount = 1,
			X = 162,
			Y = 95,
			WasDestroyed = true,
			TimeLived = 272,
		};
		var thirdSentry = new WardLog
		{
			Amount = 1,
			X = 110,
			Y = 161,
			WasDestroyed = false,
			TimeLived = 420,
		};

		// Act 
		var data = JsonSerializer.Deserialize<MatchWardLogInfo>(json);
		var reader = new WardsPlacementMapReader();
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
}

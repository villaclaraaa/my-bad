using Microsoft.Extensions.Logging;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;
using Mybad.Core.Services;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseModels.Player;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Net.Http.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardEfficiencyProvider : IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>
{
	private readonly IHttpClientFactory _factory;
	private readonly ILogger<ODotaWardEfficiencyProvider> _logger;
	private readonly IParsedMatchWardInfoService _matchService;
	private readonly IWardService _wardService;

	public ODotaWardEfficiencyProvider(
		IHttpClientFactory factory,
		ILogger<ODotaWardEfficiencyProvider> logger,
		IParsedMatchWardInfoService matchService,
		IWardService wardService)
	{
		_factory = factory;
		_logger = logger;
		_matchService = matchService;
		_wardService = wardService;
	}

	public async Task<WardsEfficiencyResponse> GetInfoAsync(WardsEfficiencyRequest request)
	{
		return (WardsEfficiencyResponse)await GetWardsLogInfo(request);
	}

	/*
	 * Flow
	 * 1. Get player Id 
	 * 2. Get last {amount} matches ID for the player
	 * 3. On every match check if the match is parsed
	 * 3.1. also check for matches for player that was parsed but no in the last {amount} matches
	 * 4. If yes - then get wards info for match
	 * 4.1. if no - ask to parse and save the state somewhere
	 * 5. Wait for all matches to be checked 
	 * 6. return
	 */
	private async Task<BaseResponse> GetWardsLogInfo(WardsEfficiencyRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		//var http = _factory.CreateClient("ODota");
		//var response = await http.GetFromJsonAsync<List<RecentMatch>>($"players/136996088/recentMatches");

		//if (response == null)
		//{
		//	throw new InvalidOperationException();
		//}
		//var reader = new WardsPlacementMapReader();

		var response = new WardsEfficiencyResponse
		{
			AccountId = request.AccountId
		};
		var localWards = await _wardService.GetAllForAccountAsync(request.AccountId);
		var localWardsList = localWards.ToList();
		try
		{
			var http = _factory.CreateClient("ODota");

			// Get recent matches
			var recentMatchesResponse = await http.GetFromJsonAsync<List<RecentMatch>>($"players/{request.AccountId}/recentMatches");
			if (recentMatchesResponse == null)
			{
				throw new NullReferenceException($"Failed to get recent matches for player {request.AccountId}.");
			}

			/* Here we start 
			 * 1. Check what recent matches was already saved into db
			 * 2. Then we add this info into response
			 * 3. Check about recent matches that was not in db if they are parsed
			 * 4. Retrieve parsed info
			 * 5. Add info into response
			 */
			var reader = new WardsPlacementMapReader();
			foreach (var match in recentMatchesResponse)
			{
				if (!await _matchService.IsMatchParsedAsync(matchId: match.MatchId, accountId: request.AccountId))
				{
					var matchDetailsResponse = await http.GetFromJsonAsync<MatchWardLogInfo>($"matches/{match.MatchId}");

					// Check if response is not valid or the match has not been parsed then skip.
					if (matchDetailsResponse == null || !matchDetailsResponse.Oddata.IsMatchParsed)
					{
						// TODO - here null check
						continue;
					}

					var obses = reader.ConvertWardsLogMatch(matchDetailsResponse, request.AccountId).obses;
					// loop through wards for match
					foreach (var obs in obses)
					{
						var ward = new Core.DomainModels.WardModel
						{
							Amount = 1,
							PosX = obs.X,
							PosY = obs.Y,
							TimeLivedSeconds = obs.TimeLived,
							WasDestroyed = obs.WasDestroyed,
							MatchId = match.MatchId,
							AccountId = request.AccountId,
						};

						// add into db
						await _wardService.AddAsync(ward);
						// add into local list
						localWardsList.Add(ward);
					}

					// add match into parsed matches
					await _matchService.AddAsync(match.MatchId, request.AccountId, DateTimeOffset.FromUnixTimeSeconds(match.StartTimeSecondsEpoch).DateTime);
					response.IncludedMatches.Add(match.MatchId);
				}
			}

			// generate efficiency for wards
			foreach (var w in localWardsList)
			{
				response.ObserverWards.Add(new WardEfficiency
				{
					X = w.PosX,
					Y = w.PosY,
					Amount = w.Amount,
					AverageTimeLived = w.TimeLivedSeconds,
					EfficiencyScore = (float)Math.Round(w.TimeLivedSeconds / 360d, 1),
				});
			}

			return response;

		}
		catch (Exception ex)
		{
			_logger.LogWarning("Exception when fetching data in {Method}, {ex}", nameof(ODotaWardEfficiencyProvider), ex);
			throw;
		}




		///* Testing only single match from local storage
		// * Code above will probably be used later. 
		// */
		//try
		//{
		//	var di = Directory.CreateDirectory("Data");
		//	using var sr = new StreamReader(Path.Combine(di.FullName, "8522407623.txt"));
		//	var json = await sr.ReadToEndAsync();
		//	var apiResponse = JsonSerializer.Deserialize<MatchWardLogInfo>(json);

		//	var reader = new WardsPlacementMapReader();
		//	var obses = reader.ConvertWardsLogMatch(apiResponse!, request.AccountId).obses;

		//	if (!obses.Any())
		//	{
		//		throw new InvalidOperationException();
		//	}

		//	/*
		//	 * Finding multiple occurring of the approx. same wards
		//	 * And returning their average position
		//	 */
		//	int deviation = 2;
		//	var groups = new List<List<WardLog>>();

		//	foreach (var obs in obses)
		//	{
		//		// Find if point is near an existing group
		//		var existingGroup = groups.FirstOrDefault(g =>
		//			g.Any(q => Math.Abs(q.X - obs.X) <= deviation && Math.Abs(q.Y - obs.Y) <= deviation));

		//		if (existingGroup != null)
		//		{
		//			existingGroup.Add(obs);
		//		}
		//		else
		//		{
		//			groups.Add([obs]);
		//		}
		//	}

		//	// build final list with averaged points
		//	var result = groups.Select(g =>
		//	{
		//		double avgX = g.Average(p => p.X);
		//		double avgY = g.Average(p => p.Y);
		//		double avgTime = g.Average(p => p.TimeLived);
		//		/* 
		//		* Now the calcucating efficiency score
		//		*/
		//		return new WardEfficiency
		//		{
		//			X = (int)Math.Round(avgX),
		//			Y = (int)Math.Round(avgY),
		//			Amount = g.Count,
		//			AverageTimeLived = (int)Math.Round(avgTime),
		//			EfficiencyScore = (float)Math.Round(avgTime / 360, 1),
		//		};
		//	}).ToList();


		//	var response = new WardsEfficiencyResponse
		//	{
		//		Id = 1,
		//		ObserverWards = result
		//	};
		//	return response;
		//}
		//catch (Exception ex)
		//{
		//	Console.WriteLine(ex.Message);
		//	throw;
		//}
	}
}

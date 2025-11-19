using Microsoft.Extensions.Logging;
using Mybad.Core;
using Mybad.Core.DomainModels;
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
	private readonly static WardsPlacementMapReader _reader = new();

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
		return (WardsEfficiencyResponse)await GetWardsEffifiency(request);
	}

	/// <summary>
	/// Gets wards efficiency for a given account.
	/// Takes all locally stored wards and fetches recent matches that are parsed in ODOta api.
	/// </summary>
	/// <param name="request">Request instance.</param>
	/// <returns>Returns <see cref="WardsEfficiencyResponse"/> instance.</returns>
	/// <exception cref="NullReferenceException"></exception>
	private async Task<BaseResponse> GetWardsEffifiency(WardsEfficiencyRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		var response = new WardsEfficiencyResponse
		{
			AccountId = request.AccountId
		};

		/* 
		 * Here we start 
		 * 1. Get wards already stored in db 
		 * 2. Get recent matches list
		 * 3. For each recent match check if the info from it was not already added into db (by ParsedMatchWardInfoService)
		 * 4. If not, fetch MatchWardLogInfo from ODota Api
		 * 5. If the match is parsed, extract wards info and store into db
		 * 6. COmposite response
		*/
		var localWards = await _wardService.GetAllForAccountAsync(request.AccountId);
		var localWardsList = localWards.ToList();
		//try
		//{
		var http = _factory.CreateClient("ODota");

		// Pinging ODota API to check if it's reachable
		var ping = await http.GetAsync("health");
		if (!ping.IsSuccessStatusCode)
		{
			response.Errors.Add("OpenDota API is not reachable.");
			response.ObserverWards = localWardsList.Select(w => new WardEfficiency
			{
				X = w.PosX,
				Y = w.PosY,
				Amount = w.Amount,
				AverageTimeLived = w.TimeLivedSeconds,
				EfficiencyScore = (float)Math.Round(w.TimeLivedSeconds / 360d, 1),
			}).ToList();
			return response;
		}

		// Get recent matches
		var recentMatchesResponse = await http.GetFromJsonAsync<List<RecentMatch>>($"players/{request.AccountId}/recentMatches");
		if (recentMatchesResponse == null)
		{
			//throw new NullReferenceException($"Failed to get recent matches for player {request.AccountId}.");
			response.Errors.Add($"Failed to get recent matches for account {request.AccountId}.");
			response.ObserverWards = localWardsList.Select(w => new WardEfficiency
			{
				X = w.PosX,
				Y = w.PosY,
				Amount = w.Amount,
				AverageTimeLived = w.TimeLivedSeconds,
				EfficiencyScore = (float)Math.Round(w.TimeLivedSeconds / 360d, 1),
			}).ToList();
			return response;
		}

		var obses = new List<WardLog>();
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

				var obsesPerMatch = _reader.ConvertWardsLogMatch(matchDetailsResponse, request.AccountId).obses;
				obsesPerMatch.Sort();
				var merged = new List<WardLog>(obsesPerMatch.Count);

				int i = 0, deviation1 = 2;
				while (i < obsesPerMatch.Count)
				{
					// Start with current element
					var current = obsesPerMatch[i];

					int j = i + 1;

					// Merge all consecutive items within deviation
					while (j < obsesPerMatch.Count &&
						   Math.Abs(current.X - obsesPerMatch[j].X) <= deviation1 &&
						   Math.Abs(current.Y - obsesPerMatch[j].Y) <= deviation1)
					{
						current.X = (current.X + obsesPerMatch[j].X) / 2;
						current.Y = (current.Y + obsesPerMatch[j].Y) / 2;
						current.TimeLived += obsesPerMatch[j].TimeLived;
						current.Amount++;
						current.WasDestroyed = current.WasDestroyed || obsesPerMatch[j].WasDestroyed;
						j++;
					}

					merged.Add(current);

					// Skip over the merged items
					i = j;
				}

				obsesPerMatch = merged;

				await _wardService.AddRangeAsync(obsesPerMatch.Select(obs => new Core.DomainModels.WardModel
				{
					Amount = obs.Amount,
					PosX = obs.X,
					PosY = obs.Y,
					TimeLivedSeconds = obs.TimeLived,
					WasDestroyed = obs.WasDestroyed,
					MatchId = match.MatchId,
					AccountId = request.AccountId,
				}));

				// add match into parsed matches
				await _matchService.AddAsync(match.MatchId, request.AccountId, DateTimeOffset.FromUnixTimeSeconds(match.StartTimeSecondsEpoch).DateTime);
				response.IncludedMatches.Add(match.MatchId);
			}
		}

		var newWardsList = await _wardService.GetAllForAccountAsync(request.AccountId);

		int deviation = 2;
		var finalWards = new List<WardEfficiency>();

		var wardsList = newWardsList.ToList();
		wardsList.Sort();

		var merged1 = new List<WardModel>(wardsList.Count);

		int i1 = 0;
		while (i1 < wardsList.Count)
		{
			// Start with current element
			var current = wardsList[i1];

			int j1 = i1 + 1;

			// Merge all consecutive items within deviation
			while (j1 < wardsList.Count &&
				   Math.Abs(current.PosX - wardsList[j1].PosX) <= deviation &&
				   Math.Abs(current.PosY - wardsList[j1].PosY) <= deviation)
			{
				current.PosX = (current.PosX + wardsList[j1].PosX) / 2;
				current.PosY = (current.PosY + wardsList[j1].PosY) / 2;
				current.TimeLivedSeconds += wardsList[j1].TimeLivedSeconds;
				current.Amount++;
				current.WasDestroyed = current.WasDestroyed || wardsList[j1].WasDestroyed;
				j1++;
			}

			merged1.Add(current);

			// Skip over the merged items
			i1 = j1;
		}

		wardsList = merged1;

		response.ObserverWards = wardsList.Select(w => new WardEfficiency
		{
			X = w.PosX,
			Y = w.PosY,
			Amount = w.Amount,
			AverageTimeLived = w.TimeLivedSeconds / w.Amount,
			EfficiencyScore = (float)Math.Round((w.TimeLivedSeconds / 360d), 1),
		}).ToList();

		return response;

		//}
		//catch (Exception ex)
		//{
		//	_logger.LogWarning("Exception when fetching data in {Method}, {ex}", nameof(ODotaWardEfficiencyProvider), ex);
		//	throw;
		//}




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

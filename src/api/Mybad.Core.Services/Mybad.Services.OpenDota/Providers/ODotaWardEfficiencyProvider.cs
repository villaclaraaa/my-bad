using System.Collections.Concurrent;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Mybad.Core;
using Mybad.Core.DomainModels;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;
using Mybad.Core.Services;
using Mybad.Core.Utility;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseModels.Player;
using Mybad.Services.OpenDota.ApiResponseReaders;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardEfficiencyProvider : IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>
{
	private readonly IHttpClientFactory _factory;
	private readonly ILogger<ODotaWardEfficiencyProvider> _logger;
	private readonly IParsedMatchWardInfoService _matchService;
	private readonly IWardService _wardService;
	private readonly static WardsConverter _reader = new();

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

	/// <inheritdoc />
	public async Task<WardsEfficiencyResponse> GetInfoAsync(WardsEfficiencyRequest request)
	{
		return await GetWardsEffifiency(request);
	}

	/// <summary>
	/// Gets wards efficiency for a given account.
	/// Takes all locally stored wards and fetches recent matches that are parsed in ODOta api.
	/// </summary>
	/// <param name="request">Request instance.</param>
	/// <returns>Returns <see cref="WardsEfficiencyResponse"/> instance.</returns>
	/// <exception cref="NullReferenceException"></exception>
	/// *
	/// * Here we start
	///	* 1. Get wards already stored in db
	/// * 2. Get recent matches list
	/// * 3. For each recent match check if the info from it was not already added into db(by ParsedMatchWardInfoService)
	/// * 4. If not, fetch MatchWardLogInfo from ODota Api
	/// * 5. If the match is parsed, extract wards info and store into db
	/// * 6. COmposite response
	/// *
	private async Task<WardsEfficiencyResponse> GetWardsEffifiency(WardsEfficiencyRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		var response = new WardsEfficiencyResponse(request.AccountId);

		var localWards = await _wardService.GetAllForAccountAsync(request.AccountId);

		var http = _factory.CreateClient("ODota");

		// Get recent matches
		var rMatchesRequest = await http.GetAsync($"players/{request.AccountId}/recentMatches");
		if (!rMatchesRequest.IsSuccessStatusCode)
		{
			_logger.LogWarning("Failed to get recent matches for account {AccountId}. Status code: {StatusCode}", request.AccountId, rMatchesRequest.StatusCode);
			response.Errors.Add($"Failed to get recent matches for account {request.AccountId}. Status code: {rMatchesRequest.StatusCode}");
			response.ObserverWards = [.. localWards.Select(w => ConvertToWardEfficiency(w))];
			return response;
		}
		var recentMatchesResponse = await rMatchesRequest.Content.ReadFromJsonAsync<List<RecentMatch>>();
		if (recentMatchesResponse == null)
		{
			response.Errors.Add($"Recent matches response could not be properly read. Account id {request.AccountId}.");
			response.ObserverWards = [.. localWards.Select(w => ConvertToWardEfficiency(w))];
			return response;
		}

		/* 
		 * Code below is using Parallel foreach to make foreach loop in parallel to be fast.
		 * Uncomment this and comment code below this to make it work in parallel.
		 * 
		 * So basically the flow here is:
		 * Remove already parsed matches from list.
		 * For the rest call the http to get data for each match in parallel.
		 * Add the wards info into concurrentBags (and parsedMatches too).
		 * In the end add bulk of wards info and matches into storage using services.AddRangeAsync() methods.
		 * Then common create and return response.
		 */

		// First remove already parsed matches. This is in different loop because we can not call dbcontext in parallel.
		// Only exception is creating own dbcontext in method, but I do not want to do this.
		int i = 0;
		while (i < recentMatchesResponse.Count)
		{
			if (await _matchService.IsMatchParsedAsync(recentMatchesResponse[i].MatchId, request.AccountId))
			{
				recentMatchesResponse.Remove(recentMatchesResponse[i]);
				continue;
			}
			i++;
		}
		// some thread safe lists.
		var obses = new ConcurrentBag<WardModel>();
		var errors = new ConcurrentBag<string>();
		var includedMatches = new ConcurrentBag<ParsedMatchWardInfoModel>();
		await Parallel.ForEachAsync(recentMatchesResponse, async (match, _) =>
		{
			var matchRequest = await http.GetAsync($"matches/{match.MatchId}");
			if (!matchRequest.IsSuccessStatusCode)
			{
				_logger.LogWarning("Failed to fetch match {MatchId} for account {AccountId}. Status code: {StatusCode}", match.MatchId, request.AccountId, matchRequest.StatusCode);
				errors.Add($"Failed to fetch match {match.MatchId}.");
				return;
			}

			var matchDetailsResponse = await matchRequest.Content.ReadFromJsonAsync<MatchWardLogInfo>();
			// Check if response is not valid or the match has not been parsed then skip.
			if (matchDetailsResponse == null || !matchDetailsResponse.Oddata.IsMatchParsed)
			{
				return;
			}

			var obsesPerMatch = _reader.ConvertWardsToWardModel(matchDetailsResponse, request.AccountId, match.MatchId, true).GetApproximatedList();
			foreach (var o in obsesPerMatch)
			{
				obses.Add(o);
			}

			var playerInfo = matchDetailsResponse.Players.FirstOrDefault(x => x.AccountId == request.AccountId)
				?? throw new InvalidOperationException();
			var isPlayerRadiant = playerInfo.Slot < 128;
			var didWin = isPlayerRadiant == matchDetailsResponse.IsRadiantWin;
			// add wards per match into db and add matchId into parsed matches
			includedMatches.Add(new ParsedMatchWardInfoModel(match.MatchId, request.AccountId, isRadiantPlayer: isPlayerRadiant, isWonMatch: didWin, DateTimeOffset.FromUnixTimeSeconds(match.StartTimeSecondsEpoch).UtcDateTime));
		});

		await _wardService.AddRangeAsync([.. obses]);
		await _matchService.AddRangeAsync(includedMatches.ToList());

		// Finally get updated wards list from storage and compose response
		var newWardsList = await _wardService.GetAllForAccountAsync(request.AccountId);
		var wardsList = newWardsList.ToList().GetApproximatedList();
		response.ObserverWards = [.. wardsList.Select(w => ConvertToWardEfficiency(w))];
		response.Errors = [.. errors];
		response.IncludedMatches = (ICollection<long>)await _matchService.GetParsedMatchesForAccountAsync(request.AccountId);
		/* */


		/* 
		 * Code below is using async/await but synchronoulsy foreach
		 * Uncomment this and comment code above to swap.
		 * 
		 * Code flow is next:
		 * foreach match
		 * check if it is not parsed.
		 * If not then call the http to get data for each match in parallel.
		 * Convert wards for single match and add a bulk into storage.
		 * Add match parsed into storage.
		 * In the end creating common response.
		 */
		//foreach (var match in recentMatchesResponse)
		//{
		//	if (await _matchService.IsMatchParsedAsync(matchId: match.MatchId, accountId: request.AccountId))
		//	{
		//		continue;
		//	}

		//	var matchRequest = await http.GetAsync($"matches/{match.MatchId}");
		//	if (!matchRequest.IsSuccessStatusCode)
		//	{
		//		_logger.LogWarning("Failed to fetch match {MatchId} for account {AccountId}. Status code: {StatusCode}", match.MatchId, request.AccountId, matchRequest.StatusCode);
		//		response.Errors.Add($"Failed to fetch match {match.MatchId}.");
		//		continue;
		//	}

		//	var matchDetailsResponse = await matchRequest.Content.ReadFromJsonAsync<MatchWardLogInfo>();

		//	// Check if response is not valid or the match has not been parsed then skip.
		//	if (matchDetailsResponse == null || !matchDetailsResponse.Oddata.IsMatchParsed)
		//	{
		//		continue;
		//	}

		//	var obsesPerMatch = _reader.ConvertWardsToWardModel(matchDetailsResponse, request.AccountId, match.MatchId, true);
		//	obsesPerMatch = obsesPerMatch.GetApproximatedList();


		//	// add wards per match into db and add matchId into parsed matches
		//	await _wardService.AddRangeAsync(obsesPerMatch);
		//	await _matchService.AddAsync(match.MatchId, request.AccountId, DateTimeOffset.FromUnixTimeSeconds(match.StartTimeSecondsEpoch).UtcDateTime);
		//}

		// Finally get updated wards list from storage and compose response
		//var newWardsList = await _wardService.GetAllForAccountAsync(request.AccountId);
		//var wardsList = newWardsList.ToList().GetApproximatedList();
		//response.ObserverWards = [.. wardsList.Select(w => ConvertToWardEfficiency(w))];
		//response.IncludedMatches = (ICollection<long>)await _matchService.GetParsedMatchesForAccountAsync(request.AccountId);
		/* */

		return response;
	}

	private static WardEfficiency ConvertToWardEfficiency(WardModel ward)
	{
		var maxtime = ward.Amount * 360;
		float timeRatio = (float)ward.TimeLivedSeconds / maxtime;
		return new WardEfficiency()
		{
			X = ward.PosX,
			Y = ward.PosY,
			Amount = ward.Amount,
			AverageTimeLived = ward.TimeLivedSeconds / ward.Amount,
			EfficiencyScore = (float)Math.Round(Math.Clamp(timeRatio, 0.0f, 1.0f), 2),
			IsRadiantSide = ward.IsRadiantSide,
		};
	}
}

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

		var http = _factory.CreateClient("ODota");

		// Pinging ODota API to check if it's reachable
		var ping = await http.GetAsync("health");
		if (!ping.IsSuccessStatusCode)
		{
			response.Errors.Add("OpenDota API is not reachable.");
			response.ObserverWards = [.. localWardsList.Select(w => ConvertToWardEfficiency(w))];
			return response;
		}

		// Get recent matches
		var recentMatchesResponse = await http.GetFromJsonAsync<List<RecentMatch>>($"players/{request.AccountId}/recentMatches");
		if (recentMatchesResponse == null)
		{
			response.Errors.Add($"Failed to get recent matches for account {request.AccountId}.");
			response.ObserverWards = [.. localWardsList.Select(w => ConvertToWardEfficiency(w))];
			return response;
		}

		var obses = new List<WardLog>();
		foreach (var match in recentMatchesResponse)
		{
			if (await _matchService.IsMatchParsedAsync(matchId: match.MatchId, accountId: request.AccountId))
			{
				continue;
			}

			var matchRequest = await http.GetAsync($"matches/{match.MatchId}");
			if (!matchRequest.IsSuccessStatusCode)
			{
				_logger.LogWarning("Failed to fetch match {MatchId} for account {AccountId}. Status code: {StatusCode}", match.MatchId, request.AccountId, matchRequest.StatusCode);
				response.Errors.Add($"Failed to fetch match {match.MatchId}.");
				continue;
			}

			//var matchDetailsResponse = await http.GetFromJsonAsync<MatchWardLogInfo>($"matches/{match.MatchId}");
			var matchDetailsResponse = await matchRequest.Content.ReadFromJsonAsync<MatchWardLogInfo>();

			// Check if response is not valid or the match has not been parsed then skip.
			if (matchDetailsResponse == null || !matchDetailsResponse.Oddata.IsMatchParsed)
			{
				// TODO - here null check
				continue;
			}

			var obsesPerMatch = _reader.ConvertWardsToWardModel(matchDetailsResponse, request.AccountId, match.MatchId, true);
			obsesPerMatch = obsesPerMatch.GetApproximatedList();

			await _wardService.AddRangeAsync(obsesPerMatch);

			// add match into parsed matches
			await _matchService.AddAsync(match.MatchId, request.AccountId, DateTimeOffset.FromUnixTimeSeconds(match.StartTimeSecondsEpoch).DateTime);
			response.IncludedMatches.Add(match.MatchId);

		}

		var newWardsList = await _wardService.GetAllForAccountAsync(request.AccountId);
		var wardsList = newWardsList.ToList().GetApproximatedList();
		response.ObserverWards = [.. wardsList.Select(w => ConvertToWardEfficiency(w))];

		return response;
	}

	private static WardEfficiency ConvertToWardEfficiency(WardModel ward) =>
		new WardEfficiency()
		{
			X = ward.PosX,
			Y = ward.PosY,
			Amount = ward.Amount,
			AverageTimeLived = ward.TimeLivedSeconds / ward.Amount,
			EfficiencyScore = (float)Math.Round((ward.TimeLivedSeconds / 360d), 1),
		};
}

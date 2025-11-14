using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Net.Http.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardsSingleMatchProvider : IInfoProvider<WardLogSingleMatchRequest, WardsLogMatchResponse>
{
	private static string _urlPath = "https://api.opendota.com/api/";

	public async Task<WardsLogMatchResponse> GetInfoAsync(WardLogSingleMatchRequest request)
	{
		var accountId = request.AccountId;
		try
		{
			using var http = new HttpClient();
			var apiResponse = await http.GetFromJsonAsync<MatchWardLogInfo>(_urlPath + $"matches/{request.MatchId}");

			if (apiResponse == null)
			{
				throw new InvalidOperationException();
			}

			var reader = new WardsPlacementMapReader();

			var (obses, sens) = reader.ConvertWardsLogMatch(apiResponse, accountId);
			var response = new WardsLogMatchResponse()
			{
				Id = 1,
				ObserverWardsLog = obses,
				SentryWardsLog = sens,
				AccountId = accountId
			};

			return response;

		}
		catch (Exception)
		{
			throw;
		}
	}
}

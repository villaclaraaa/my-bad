using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Net.Http.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardsSingleMatchProvider : IInfoProvider<WardLogSingleMatchRequest, WardsLogMatchResponse>
{
	private readonly IHttpClientFactory _factory;

	public ODotaWardsSingleMatchProvider(IHttpClientFactory factory)
	{
		_factory = factory;
	}

	public async Task<WardsLogMatchResponse> GetInfoAsync(WardLogSingleMatchRequest request)
	{
		var accountId = request.AccountId;
		try
		{
			var http = _factory.CreateClient("ODota");
			var apiResponse = await http.GetFromJsonAsync<MatchWardLogInfo>($"matches/{request.MatchId}");

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

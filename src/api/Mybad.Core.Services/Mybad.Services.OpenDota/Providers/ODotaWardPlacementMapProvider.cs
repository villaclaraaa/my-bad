using Microsoft.Extensions.Logging;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Net.Http.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardPlacementMapProvider : IInfoProvider<WardMapRequest, WardsMapPlacementResponse>
{
	private readonly IHttpClientFactory _factory;
	private readonly ILogger<ODotaWardPlacementMapProvider> _logger;

	public ODotaWardPlacementMapProvider(IHttpClientFactory factory, ILogger<ODotaWardPlacementMapProvider> logger)
	{
		_factory = factory;
		_logger = logger;
	}

	public async Task<WardsMapPlacementResponse> GetInfoAsync(WardMapRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		var response = new WardsMapPlacementResponse(request.AccountId);
		var http = _factory.CreateClient("ODota");

		// Get recent matches
		var apiResponse = await http.GetAsync($"players/{request.AccountId}/wardmap?having=100");
		if (!apiResponse.IsSuccessStatusCode)
		{
			_logger.LogWarning("Failed to get recent matches for account {AccountId}. Status code: {StatusCode}", request.AccountId, apiResponse.StatusCode);
			response.Errors.Add($"Failed to get recent matches for account {request.AccountId}. Status code: {apiResponse.StatusCode}");
			return response;
		}

		var wardsMap = await apiResponse.Content.ReadFromJsonAsync<WardPlacementMap>();
		if (wardsMap == null)
		{
			_logger.LogWarning("Failed to read api response.");
			response.Errors.Add($"Failed to read from OpenDota.");
			return response;
		}

		var reader = new WardsConverter();
		(response.ObserverWards, response.SentryWards) = reader.ConvertToWardList(wardsMap);
		return response;
	}
}

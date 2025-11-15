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

	public ODotaWardPlacementMapProvider(IHttpClientFactory factory)
	{
		_factory = factory;
	}

	public async Task<WardsMapPlacementResponse> GetInfoAsync(WardMapRequest request)
	{
		var http = _factory.CreateClient("ODota");

		try
		{
			var apiResponse = await http.GetFromJsonAsync<WardPlacementMap>($"players/{request.AccountId}/wardmap?having=100");

			if (apiResponse == null)
			{
				throw new InvalidOperationException();
			}

			var reader = new WardsPlacementMapReader();
			var (obses, sens) = reader.ConvertToWardList(apiResponse);
			var response = new WardsMapPlacementResponse
			{
				Id = 1,
				ObserverWards = obses,
				SentryWards = sens,
				AccountId = request.AccountId
			};
			return response;
		}
		catch (Exception)
		{
			throw;
		}
	}
}

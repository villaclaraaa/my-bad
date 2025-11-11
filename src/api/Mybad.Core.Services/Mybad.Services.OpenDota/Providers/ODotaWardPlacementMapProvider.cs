using System.Net.Http.Json;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardPlacementMapProvider : IInfoProvider<WardMapRequest, WardsMapPlacementResponse>
{
	private static string _urlPath = "https://api.opendota.com/api/";

	public async Task<WardsMapPlacementResponse> GetInfo(WardMapRequest request)
	{
		using var http = new HttpClient();
		//var response = await http.GetFromJsonAsync<WardsInfo>(_urlPath + $"players/136996088/matches?limit={request.MatchesCount}");

		try
		{
			var apiResponse = await http.GetFromJsonAsync<WardPlacementMap>(_urlPath + $"players/136996088/wardmap?having=100");

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
			};
			return response;
		}
		catch (Exception)
		{
			throw;
		}
	}
}

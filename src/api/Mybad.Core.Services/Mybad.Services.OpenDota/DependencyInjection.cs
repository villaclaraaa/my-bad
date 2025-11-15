using Microsoft.Extensions.DependencyInjection;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.Providers;

namespace Mybad.Services.OpenDota;

public static class DependencyInjection
{
	public static IServiceCollection AddODotaServices(this IServiceCollection services)
	{
		// add http client for odota
		services.AddHttpClient("ODota", client =>
		{
			client.BaseAddress = new Uri("https://api.opendota.com/api/");
		});

		// add info providers
		services.AddODotaInfoProviders();

		return services;
	}

	private static IServiceCollection AddODotaInfoProviders(this IServiceCollection services)
	{
		services.AddScoped<IInfoProvider<WardMapRequest, WardsMapPlacementResponse>, ODotaWardPlacementMapProvider>();
		services.AddScoped<IInfoProvider<WardLogSingleMatchRequest, WardsLogMatchResponse>, ODotaWardsSingleMatchProvider>();
		services.AddScoped<IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>, ODotaWardEfficiencyProvider>();
		return services;
	}
}

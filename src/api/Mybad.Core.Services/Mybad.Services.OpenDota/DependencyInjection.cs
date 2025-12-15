using Microsoft.Extensions.DependencyInjection;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.Cachers;
using Mybad.Services.OpenDota.Providers;

namespace Mybad.Services.OpenDota;

public static class DependencyInjection
{
	public static IServiceCollection AddODotaServices(this IServiceCollection services)
	{
		// add http client configured for ODota
		services.AddHttpClient("ODota", client =>
		{
			client.BaseAddress = new Uri("https://api.opendota.com/api/");
		});

		// adding info providers and cachers
		services.AddODotaInfoProviders();
		services.AddODotaCachers();

		return services;
	}

	private static IServiceCollection AddODotaInfoProviders(this IServiceCollection services)
	{
		services.AddScoped<IInfoProvider<WardMapRequest, WardsMapPlacementResponse>, ODotaWardPlacementMapProvider>();
		services.AddScoped<IInfoProvider<WardLogSingleMatchRequest, WardsLogMatchResponse>, ODotaWardsSingleMatchProvider>();
		services.AddScoped<IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>, ODotaWardEfficiencyProvider>();
		services.AddScoped<IInfoProvider<PlayerInfoRequest, PlayerInfoResponse>, ODotaPlayerInfoProvider>();

		return services;
	}

	private static IServiceCollection AddODotaCachers(this IServiceCollection services)
	{
		services.AddScoped<ODotaHeroMatchupCacher>();

		return services;
	}
}

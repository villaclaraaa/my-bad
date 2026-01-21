using Microsoft.Extensions.DependencyInjection;
using Mybad.Core.Services;
using Mybad.Storage.DB.Services;

namespace Mybad.Storage.DB;

public static class DependencyInjection
{
	public static IServiceCollection AddDbServices(this IServiceCollection services)
	{
		services.AddScoped<IWardService, WardsService>();
		services.AddScoped<IMatchupService, MatchupService>();
		services.AddScoped<ICheckedMatchesService, CheckedMatchesService>();
		services.AddScoped<IParsedMatchWardInfoService, ParsedMatchWardInfoService>();
		services.AddScoped<IHeroMatchesService, HeroMatchesService>();

		return services;
	}
}

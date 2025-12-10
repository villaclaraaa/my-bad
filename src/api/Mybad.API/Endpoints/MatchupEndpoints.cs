using Microsoft.AspNetCore.Mvc;
using Mybad.API.Services;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;

namespace Mybad.API.Endpoints
{
	public static class MatchupEndpoints
	{
		public static RouteGroupBuilder MapMatchupEndpoints(this IEndpointRouteBuilder routes)
		{
			var group = routes.MapGroup("api/matchups").WithTags("Matchups");

			group.MapPost("find", FindBestHeroes);

			// Background service cacher related endpoints
			group.MapGet("startCacher", StartCacher);
			group.MapGet("stopCacher", StopCacher);

			return group;
		}

		private static IResult StopCacher(HeroMatchupCacherStatus cacherStatus)
		{
			cacherStatus.IsEnabled = false;
			return TypedResults.Ok("Cacher stopped.");
		}

		private static IResult StartCacher(HeroMatchupCacherStatus cacherStatus)
		{
			cacherStatus.IsEnabled = true;
			return TypedResults.Ok("Cacher resumed.");
		}

		private static async Task<IResult> FindBestHeroes([FromBody] HeroMatchupsRequestModel model,
			IInfoProvider<HeroMatchupRequest, HeroMatchupResponse> provider)
		{
			var request = new HeroMatchupRequest() { EnemyIds = model.EnemyIds, AllyIds = model.AllyIds };
			var response = await provider.GetInfoAsync(request);
			return TypedResults.Ok(response);
		}
	}

	internal class HeroMatchupsRequestModel
	{
		public List<int>? EnemyIds { get; set; }
		public List<int>? AllyIds { get; set; }
	}

}
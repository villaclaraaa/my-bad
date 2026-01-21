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
            var group = routes.MapGroup("api/matchups")
                .WithTags("Matchups")
                .RequireCors("AllowAngularApp");

            group.MapPost("find", FindBestHeroes)
                .Produces(200);

            // Background service cacher related endpoints
            group.MapGet("startCacher", StartCacher)
                .Produces(200)
                .Produces(StatusCodes.Status401Unauthorized)
                .AddEndpointFilter<ApiKeyEndpointFilter>();

            group.MapGet("stopCacher", StopCacher)
                .Produces(200)
                .Produces(StatusCodes.Status401Unauthorized)
                .AddEndpointFilter<ApiKeyEndpointFilter>();

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

        private static async Task<IResult> FindBestHeroes(HeroMatchupsRequestModel model,
            IInfoProvider<HeroMatchupRequest, HeroMatchupResponse> provider)
        {
            var request = new HeroMatchupRequest() { EnemyIds = model.EnemyIds, AllyIds = model.AllyIds, HeroesInPool = model.HeroesInPool };
            var response = await provider.GetInfoAsync(request);
            return TypedResults.Ok(response);
        }
    }

    internal class HeroMatchupsRequestModel
    {
        public List<int>? EnemyIds { get; set; }
        public List<int>? AllyIds { get; set; }
        public List<int>? HeroesInPool { get; set; }
    }

}
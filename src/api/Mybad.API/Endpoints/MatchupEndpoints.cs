using Microsoft.AspNetCore.Mvc;
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

            // Define ward-related endpoints here
            group.MapPost("find", FindBestHeroes);
            return group;
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

using Microsoft.AspNetCore.Mvc;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;

namespace Mybad.API.Endpoints;

public static class WardEndpoints
{
	public static RouteGroupBuilder MapWardEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("api/wards").WithTags("Wards");
		// Define ward-related endpoints here
		group.MapGet("map", GetWardsPlacementMap);
		return group;
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	private static async Task<IResult> GetWardsPlacementMap(
		[FromQuery] long accountId,
		IInfoProvider<WardMapRequest, WardsMapPlacementResponse> provider)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(accountId);

		var request = new WardMapRequest(accountId);
		var response = await provider.GetInfoAsync(request);
		return TypedResults.Ok(response);
	}
}

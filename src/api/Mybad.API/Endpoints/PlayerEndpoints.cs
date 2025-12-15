
using Microsoft.AspNetCore.Mvc;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;

namespace Mybad.API.Endpoints;

public static class PlayerEndpoints
{
	public static RouteGroupBuilder MapPlayerEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("api/players").WithTags("Players");

		// Define ward-related endpoints here
		group.MapGet("baseinfo", GetBasePlayerInfo);
		return group;
	}

	private static async Task<IResult> GetBasePlayerInfo(
		[FromQuery] long accountId,
		IInfoProvider<PlayerInfoRequest, PlayerInfoResponse> provider)
	{
		//ArgumentOutOfRangeException.ThrowIfNegative(accountId);
		if (accountId <= 0)
		{
			return TypedResults.BadRequest("AccountId must be a positive number.");
		}
		var request = new PlayerInfoRequest(accountId);
		var response = await provider.GetInfoAsync(request);
		return TypedResults.Ok(response);
	}
}

using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Services;

namespace Mybad.API.Endpoints;

public static class WardEndpoints
{
	public static RouteGroupBuilder MapWardEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("api/wards")
			.WithTags("Wards")
			.RequireCors("AllowAngularApp");

		// Define ward-related endpoints here
		group.MapGet("map", GetWardsPlacementMap)
			.Produces<WardsMapPlacementResponse>(200)
			.Produces(400);

		group.MapGet("efficiency", GetWardsEfficiency)
			.Produces<WardsEfficiencyResponse>(200)
			.Produces(400);

		group.MapDelete("efficiency/match", DeleteMatchFromEfficiency)
			.Produces(204)
			.Produces(400);

		return group;
	}

	private static async Task<IResult> DeleteMatchFromEfficiency(
		long matchId, long accountId,
		IParsedMatchWardInfoService matchesService, IWardService wardsService)
	{
		if (accountId <= 0 || matchId <= 0)
		{
			return TypedResults.BadRequest("Account id or match id must be a positive number.");
		}

		await wardsService.DeleteAllFromMatchAsync(matchId);
		await matchesService.RemoveAsync(matchId, accountId);

		return TypedResults.NoContent();
	}

	private static async Task<IResult> GetWardsEfficiency(
		long accountId,
		IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse> provider)
	{
		if (accountId <= 0)
		{
			return TypedResults.BadRequest("AccountId must be a positive number.");
		}
		var request = new WardsEfficiencyRequest(accountId);
		var response = await provider.GetInfoAsync(request);
		return TypedResults.Ok(response);
	}

	private static async Task<IResult> GetWardsPlacementMap(
		long accountId,
		IInfoProvider<WardMapRequest, WardsMapPlacementResponse> provider)
	{
		//ArgumentOutOfRangeException.ThrowIfNegative(accountId);
		if (accountId <= 0)
		{
			return TypedResults.BadRequest("AccountId must be a positive number.");
		}

		var request = new WardMapRequest(accountId);
		var response = await provider.GetInfoAsync(request);
		return TypedResults.Ok(response);
	}
}


using Microsoft.AspNetCore.Mvc;
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
		group.MapGet("map", GetWardsPlacementMap);
		group.MapGet("efficiency", GetWardsEfficiency);
		group.MapDelete("efficiency/match", DeleteMatchFromEfficiency);
		return group;
	}

	private static async Task<IResult> DeleteMatchFromEfficiency(
		[FromQuery] long matchId, [FromQuery] long accountId,
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
		[FromQuery] long accountId, [FromQuery] bool? forRadiant,
		IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse> provider)
	{
		//ArgumentOutOfRangeException.ThrowIfNegative(accountId);
		if (accountId <= 0)
		{
			return TypedResults.BadRequest("AccountId must be a positive number.");
		}
		var request = new WardsEfficiencyRequest(accountId);
		request.ForRadiantSide = forRadiant;
		var response = await provider.GetInfoAsync(request);
		return TypedResults.Ok(response);
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	private static async Task<IResult> GetWardsPlacementMap(
		[FromQuery] long accountId,
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

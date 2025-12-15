using Microsoft.Extensions.Logging;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Services.OpenDota.ApiResponseModels;
using System.Net.Http.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaPlayerInfoProvider : IInfoProvider<PlayerInfoRequest, PlayerInfoResponse>
{
	private readonly IHttpClientFactory _factory;
	private readonly ILogger<ODotaPlayerInfoProvider> _logger;


	public ODotaPlayerInfoProvider(
		IHttpClientFactory factory,
		ILogger<ODotaPlayerInfoProvider> logger)
	{
		_factory = factory;
		_logger = logger;
	}

	/// <summary>
	/// Retrieves player information from the OpenDota API for the specified account asynchronously.
	/// </summary>
	/// <param name="request">The request containing the account ID of the player whose information is to be retrieved.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PlayerInfoResponse"/> with
	/// the retrieved player information if successful; otherwise, the response includes error details.</returns>
	public async Task<PlayerInfoResponse> GetInfoAsync(PlayerInfoRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		var response = new PlayerInfoResponse(request.AccountId);
		var http = _factory.CreateClient("ODota");

		// Sending request.
		var httpRequest = await http.GetAsync($"players/{request.AccountId}");
		if (!httpRequest.IsSuccessStatusCode)
		{
			_logger.LogWarning("Failed to get player info for account {request.AccountId}. Status code: {httpRequest.StatusCode}", request.AccountId, httpRequest.StatusCode);
			response.Errors.Add($"Failed to get player info for account {request.AccountId}. Status code: {httpRequest.StatusCode}");
			return response;
		}

		// Trying to read its content.
		var content = await httpRequest.Content.ReadFromJsonAsync<PlayerModel>();
		if (content == null)
		{
			_logger.LogWarning("Could not read player data for account {request.AccountId}.", request.AccountId);
			response.Errors.Add($"Could not read player data for account {request.AccountId}");
			return response;
		}

		// Generating proper expected response.
		response.PlayerInfo = new Core.Responses.Entries.PlayerInfo
		{
			AccountId = content.Profile.AccountId,
			PersonaName = content.Profile.PersonaName,
			AvatarMediumUrl = content.Profile.AvatarMediumUrl
		};

		return response;
	}
}

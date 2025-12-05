using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseModels.Player;
using System.Net.Http.Json;

namespace OpenDotaService.Tests;

/// <summary>
/// This is the tests for ensuring that when sending some request to ODota API 
/// the correct Model is received in API response.
/// <br/>
/// The Model expected for each test is in <see cref="N:Mybad.Services.OpenDota.ApiResponseModels"/> namespace.
/// </summary>
/// <remarks>
/// THESE SHOULD NOT BE RUN EVERY TIME. IT CAN BE RUN ONLY ONCE A WHILE JUST TO ENSURE THAT API RESPONSE HAS NOT CHANGED.
/// </remarks>
public class HttpRequestsResponsesTests : IClassFixture<HttpClientFixture>
{

	private readonly HttpClient _http;
	private readonly long _playerId = 136996088L;

	public HttpRequestsResponsesTests(HttpClientFixture fixture)
	{
		_http = fixture.Client;
	}

	[Fact]
	public async Task CheckODotaHealth_Returns_200()
	{
		// Act 
		var response = await _http.GetAsync("health");

		// Assert
		Assert.True(response.IsSuccessStatusCode);
	}

	/// <summary>
	/// Should get <see cref="WardPlacementMap"/> object.
	/// </summary>
	[Fact]
	public async Task GetWardMapRequest_Returns_WardPlacementMap()
	{
		// Act
		var response = await _http.GetAsync($"players/{_playerId}/wardmap?having=10");
		response.EnsureSuccessStatusCode();
		var result = await response.Content.ReadFromJsonAsync<WardPlacementMap>();

		// Assert
		Assert.NotNull(result);
		Assert.IsType<WardPlacementMap>(result);
	}

	[Fact]
	public async Task GetRecentMacthes_Returns_RecentMatches()
	{
		// Act
		var response = await _http.GetAsync($"players/{_playerId}/recentMatches");
		response.EnsureSuccessStatusCode();
		var result = await response.Content.ReadFromJsonAsync<List<RecentMatch>>();

		// Assert
		Assert.NotNull(result);
		Assert.IsType<List<RecentMatch>>(result);
		Assert.True(result.Count != 0);
	}

	[Fact]
	public async Task GetMatchWardInfo_Returns_MatchWardLogInfo()
	{
		// Arrange 
		var matchId = 8590369459L;

		// Act
		var response = await _http.GetAsync($"matches/{matchId}");
		response.EnsureSuccessStatusCode();
		var result = await response.Content.ReadFromJsonAsync<MatchWardLogInfo>();

		// Assert
		Assert.NotNull(result);
		Assert.IsType<MatchWardLogInfo>(result);
	}

	[Fact]
	public async Task GetPublicMatches_Returns_PublicMatchModel()
	{
		// Arrange 
		var minRank = 70;

		// Act
		var response = await _http.GetAsync($"publicMatches?min_rank={minRank}");
		response.EnsureSuccessStatusCode();
		var result = await response.Content.ReadFromJsonAsync<List<PublicMatchModel>>();

		// Assert
		Assert.NotNull(result);
		Assert.IsType<List<PublicMatchModel>>(result);
		Assert.True(result.Count > 0);
	}

}


public class HttpClientFixture : IDisposable
{
	public HttpClient Client { get; }

	public HttpClientFixture()
	{
		Client = new HttpClient
		{
			BaseAddress = new Uri("https://api.opendota.com/api/")
		};
	}

	public void Dispose()
	{
		Client.Dispose();
	}
}
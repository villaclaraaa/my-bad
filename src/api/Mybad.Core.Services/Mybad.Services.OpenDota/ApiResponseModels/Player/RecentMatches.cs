using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels.Player;

internal class RecentMatches
{
	public List<RecentMatch> Matches { get; set; } = new List<RecentMatch>();
}

internal class RecentMatch
{
	/// <summary>
	/// Gets or sets match id.
	/// </summary>
	[JsonPropertyName("match_id")]
	public long MatchId { get; set; }

	/// <summary>
	/// Gets or sets line id for the maatch. 
	/// <br/>It has <c>null</c> if match was not parsed yet.
	/// </summary>
	[JsonPropertyName("lane")]
	public int? Lane { get; set; }
}

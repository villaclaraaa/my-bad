using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels;

internal class WardPlacementMap
{
	[JsonPropertyName("obs")]
	public Dictionary<string, Dictionary<string, int>> Obs { get; set; } = [];

	[JsonPropertyName("sen")]
	public Dictionary<string, Dictionary<string, int>> Sen { get; set; } = [];
}

using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels;

internal class PlayerModel
{
	[JsonPropertyName("profile")]
	public PlayerProfile Profile { get; set; } = null!;
}

internal class PlayerProfile
{
	[JsonPropertyName("account_id")]
	public long AccountId { get; set; }

	[JsonPropertyName("personaname")]
	public string PersonaName { get; set; } = null!;

	[JsonPropertyName("avatarmedium")]
	public string AvatarMediumUrl { get; set; } = null!;
}

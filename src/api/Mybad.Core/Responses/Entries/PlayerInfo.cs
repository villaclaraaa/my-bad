namespace Mybad.Core.Responses.Entries;

public class PlayerInfo
{
	public long AccountId { get; set; }

	public string PersonaName { get; set; } = null!;

	public string AvatarMediumUrl { get; set; } = null!;
}

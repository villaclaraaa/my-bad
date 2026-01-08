namespace Mybad.Core.Responses.Entries;

public class ParsedMatchWardEfficiency
{
	public long MatchId { get; set; }
	public long AccountId { get; set; }
	public string HeroName { get; set; } = null!;
	public bool IsRadiantPlayer { get; set; }
	public bool IsWonMatch { get; set; }
	public DateTime PlayedAtDateUtc { get; set; } = DateTime.UtcNow;
}

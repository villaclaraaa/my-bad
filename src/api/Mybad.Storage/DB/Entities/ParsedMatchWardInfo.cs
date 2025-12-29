namespace Mybad.Storage.DB.Entities;

public class ParsedMatchWardInfo
{
	public long MatchId { get; set; }
	public long AccountId { get; set; }
	public bool IsRadiantPlayer { get; set; }
	public bool IsWonMatch { get; set; }
	public DateTime PlayedAtDateUtc { get; set; }
}

namespace Mybad.Core.DomainModels;

public class ParsedMatchWardInfoModel
{
	public ParsedMatchWardInfoModel(long matchId, long accountId, bool isRadiantPlayer, bool isWonMatch, DateTime playedAtDateUtc, int heroId)
	{
		MatchId = matchId;
		AccountId = accountId;
		IsRadiantPlayer = isRadiantPlayer;
		IsWonMatch = isWonMatch;
		PlayedAtDateUtc = playedAtDateUtc;
		HeroId = heroId;
	}

	public long MatchId { get; set; }
	public long AccountId { get; set; }
	public int HeroId { get; set; }
	public bool IsRadiantPlayer { get; set; }
	public bool IsWonMatch { get; set; }
	public DateTime PlayedAtDateUtc { get; set; } = DateTime.UtcNow;
}

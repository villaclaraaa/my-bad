namespace Mybad.Core.Requests;

public class WardLogSingleMatchRequest : BaseRequest, IAccountPiece, IMatchPiece
{
	public WardLogSingleMatchRequest(long accountId, long matchId)
	{
		AccountId = accountId;
		MatchId = matchId;
	}

	public required long MatchId { get; init; }
	public required long AccountId { get; init; }
}

namespace Mybad.Core.Requests;

public class WardLogSingleMatchRequest : BaseRequest
{
	public WardLogSingleMatchRequest(long accountId, long matchId)
	{
		AccountId = accountId;
		MatchId = matchId;
	}

	public long MatchId { get; set; }
}

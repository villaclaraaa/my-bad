namespace Mybad.Core.Requests;

public class WardLogRequest : BaseRequest, IAccountPiece
{
	public WardLogRequest(long accountId)
	{
		AccountId = accountId;
	}

	public required long AccountId { get; init; }
}

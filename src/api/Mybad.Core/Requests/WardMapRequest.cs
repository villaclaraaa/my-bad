namespace Mybad.Core.Requests;

public class WardMapRequest : BaseRequest, IAccountPiece
{
	public WardMapRequest(long accountId)
	{
		AccountId = accountId;
	}

	public required long AccountId { get; init; }
}

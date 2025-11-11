namespace Mybad.Core.Requests;

public class WardsEfficiencyRequest : BaseRequest, IAccountPiece
{
	public WardsEfficiencyRequest(int accountId)
	{
		AccountId = accountId;
	}
	public required long AccountId { get; init; }
}

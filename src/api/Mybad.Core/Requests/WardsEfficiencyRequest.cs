namespace Mybad.Core.Requests;

public class WardsEfficiencyRequest : BaseRequest
{
	public WardsEfficiencyRequest(int accountId)
	{
		AccountId = accountId;
	}
}

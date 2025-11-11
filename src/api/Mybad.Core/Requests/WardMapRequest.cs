namespace Mybad.Core.Requests;

public class WardMapRequest : BaseRequest
{
	public WardMapRequest(long accountId)
	{
		AccountId = accountId;
	}
}

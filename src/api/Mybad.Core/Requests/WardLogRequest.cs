namespace Mybad.Core.Requests;

public class WardLogRequest : BaseRequest
{
	public WardLogRequest(long accountId)
	{
		AccountId = accountId;
	}
}

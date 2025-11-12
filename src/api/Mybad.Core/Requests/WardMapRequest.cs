using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Requests;

public class WardMapRequest : BaseRequest
{
	[SetsRequiredMembers]
	public WardMapRequest(long accountId)
	{
		AccountId = accountId;
	}
}

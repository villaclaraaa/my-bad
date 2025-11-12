using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Requests;

public class WardMapRequest : BaseRequest, IAccountPiece
{
	[SetsRequiredMembers]
	public WardMapRequest(long accountId)
	{
		AccountId = accountId;
	}

	public required long AccountId { get; init; }
}

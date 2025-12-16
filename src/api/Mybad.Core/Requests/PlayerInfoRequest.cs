using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Requests;

public class PlayerInfoRequest : BaseRequest, IAccountPiece
{
	[SetsRequiredMembers]
	public PlayerInfoRequest(long accountId)
	{
		AccountId = accountId;
	}

	public long AccountId { get; init; }
}

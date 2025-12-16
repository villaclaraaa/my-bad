using Mybad.Core.Responses.Entries;
using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Responses;

public class PlayerInfoResponse : BaseResponse, IAccountPiece
{
	[SetsRequiredMembers]
	public PlayerInfoResponse(long accountId)
	{
		AccountId = accountId;
	}

	public long AccountId { get; init; }

	public PlayerInfo? PlayerInfo { get; set; }
}

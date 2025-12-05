using Mybad.Core.Responses.Entries;
using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Responses;

public class WardsMapPlacementResponse : BaseResponse, IAccountPiece
{
	[SetsRequiredMembers]
	public WardsMapPlacementResponse(long accountId)
	{
		AccountId = accountId;
	}

	public List<Ward> ObserverWards { get; set; } = [];
	public List<Ward> SentryWards { get; set; } = [];

	public required long AccountId { get; init; }
}

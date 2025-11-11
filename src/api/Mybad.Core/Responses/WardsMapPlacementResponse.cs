using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsMapPlacementResponse : BaseResponse, IAccountPiece
{
	public List<Ward> ObserverWards { get; set; } = [];
	public List<Ward> SentryWards { get; set; } = [];

	public required long AccountId { get; init; }
}

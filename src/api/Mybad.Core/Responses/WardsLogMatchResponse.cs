using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsLogMatchResponse : BaseResponse, IAccountPiece
{
	public List<WardLog> ObserverWardsLog { get; set; } = [];
	public List<WardLog> SentryWardsLog { get; set; } = [];

	public required long AccountId { get; init; }
}

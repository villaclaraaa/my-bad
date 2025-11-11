using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsMapPlacementResponse : BaseResponse
{
	public List<Ward> ObserverWards { get; set; } = [];
	public List<Ward> SentryWards { get; set; } = [];
}

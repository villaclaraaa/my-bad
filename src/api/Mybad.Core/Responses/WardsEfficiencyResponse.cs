using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsEfficiencyResponse : BaseResponse
{
	public IEnumerable<WardEfficiency> ObserverWards { get; set; } = [];
}

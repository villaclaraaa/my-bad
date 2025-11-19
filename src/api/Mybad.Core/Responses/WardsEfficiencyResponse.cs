using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsEfficiencyResponse : BaseResponse, IAccountPiece
{
	public ICollection<WardEfficiency> ObserverWards { get; set; } = [];
	public ICollection<long> IncludedMatches { get; set; } = [];
	public int TotalWardsPlaced => ObserverWards.Sum(w => w.Amount);
	public int WardsDistinctPositions => ObserverWards.Count;
	public long AccountId { get; init; }
}

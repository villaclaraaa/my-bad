using Mybad.Core.Responses.Entries;
using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Responses;

public class WardsEfficiencyResponse : BaseResponse, IAccountPiece
{
	[SetsRequiredMembers]
	public WardsEfficiencyResponse(long accountId)
	{
		AccountId = accountId;
	}

	public ICollection<WardEfficiency> ObserverWards { get; set; } = [];
	public ICollection<long> IncludedMatches { get; set; } = [];
	public int TotalWardsPlaced => ObserverWards.Sum(w => w.Amount);
	public int WardsDistinctPositions => ObserverWards.Count;
	public required long AccountId { get; init; }
}

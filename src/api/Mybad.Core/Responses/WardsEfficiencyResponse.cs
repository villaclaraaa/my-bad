using System.Diagnostics.CodeAnalysis;
using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses;

public class WardsEfficiencyResponse : BaseResponse, IAccountPiece
{
	[SetsRequiredMembers]
	public WardsEfficiencyResponse(long accountId)
	{
		AccountId = accountId;
	}

	public IEnumerable<WardEfficiency> ObserverWards { get; set; } = [];
	public IEnumerable<ParsedMatchWardEfficiency> IncludedMatches { get; set; } = [];
	public int TotalWardsPlaced => ObserverWards.Sum(w => w.Amount);
	public int WardsDistinctPositions => ObserverWards.Count();
	public required long AccountId { get; init; }
}

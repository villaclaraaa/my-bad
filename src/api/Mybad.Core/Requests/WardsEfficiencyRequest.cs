using System.Diagnostics.CodeAnalysis;

namespace Mybad.Core.Requests;

public class WardsEfficiencyRequest : BaseRequest, IAccountPiece
{
	[SetsRequiredMembers]
	public WardsEfficiencyRequest(long accountId)
	{
		AccountId = accountId;
	}

	public required long AccountId { get; init; }
	public bool? ForRadiantSide { get; set; }
}

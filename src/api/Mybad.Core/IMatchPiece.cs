namespace Mybad.Core;

/// <summary>
/// Contracts for parts of info about Match. <b>Implement this interface in request/response to have match info.</b>
/// </summary>
public interface IMatchPiece
{
	/// <summary>
	/// Gets or sets id of the match.
	/// </summary>
	public long MatchId { get; init; }
}

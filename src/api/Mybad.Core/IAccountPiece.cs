namespace Mybad.Core;

/// <summary>
/// Contracts for parts of info about Account. <b>Implement this interface in request/response to have Account info.</b>
/// </summary>
public interface IAccountPiece
{

	/// <summary>
	/// Gets or sets account id of the player.
	/// </summary>
	long AccountId { get; init; }
}

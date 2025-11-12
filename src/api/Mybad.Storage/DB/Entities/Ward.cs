namespace Mybad.Storage.DB.Entities;

public class Ward
{
	/// <summary>
	/// Gets or sets X position of the ward.
	/// </summary>
	/// <remarks>It is average positions of ward.</remarks>
	public int PosX { get; set; }

	/// <summary>
	/// Gets or sets Y position of the ward.
	/// </summary>
	/// <remarks>It is average positions of ward.</remarks>
	public int PosY { get; set; }

	/// <summary>
	/// Gets or sets amount of wards placed on this position.
	/// </summary>
	public int Amount { get; set; }

	/// <summary>
	/// Gets or sets id of the match.
	/// </summary>
	public long MatchId { get; set; }

	/// <summary>
	/// Gets or sets account id of the player who placed the ward.
	/// </summary>
	public long AccountId { get; set; }

	/// <summary>
	/// Gets or sets time lived in seconds.
	/// </summary>
	public int TimeLivedSeconds { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the ward was destroyed.
	/// </summary>
	public bool WasDestroyed { get; set; }
}

namespace Mybad.Storage.DB.Entities;

public class WardEntity
{
	public int PosX { get; set; }

	public int PosY { get; set; }

	public int Amount { get; set; }

	public long MatchId { get; set; }

	public long AccountId { get; set; }

	public int TimeLivedSeconds { get; set; }

	public bool WasDestroyed { get; set; }

	public DateTime CreatedDate { get; set; }


	public ParsedMatchWardInfo ParsedMatch { get; set; } = default!;
}

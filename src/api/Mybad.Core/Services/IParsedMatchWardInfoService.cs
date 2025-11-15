namespace Mybad.Core.Services;

/// <summary>
/// Contracts for service that manages parsed match ward info.
/// </summary>
public interface IParsedMatchWardInfoService
{
	/// <summary>
	/// Adds a parsed match record.
	/// </summary>
	/// <param name="matchId">Match id.</param>
	/// <param name="playedAtDateUtc">DateTime when match played in Utc.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task AddAsync(long matchId, DateTime playedAtDateUtc);

	/// <summary>
	/// Deletes the parsed match record.
	/// </summary>
	/// <param name="matchId">Match id to delete.</param>
	/// <returns></returns>
	Task RemoveAsync(long matchId);

	/// <summary>
	/// Checks if the match is already parsed (meaning info about wards are added into other table).
	/// </summary>
	/// <param name="matchId">Match id to check.</param>
	/// <returns>Task representing asynchronous operation. 
	/// Task result contains <b>true</b> is the match is parsed and info is added into other tables, otherwise <b>false</b>.</returns>
	Task<bool> IsMatchParsedAsync(long matchId);
}

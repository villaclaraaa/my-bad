using Mybad.Core.DomainModels;

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
	/// <param name="accountId">Account id.</param>
	/// <param name="playedAtDateUtc">DateTime when match played in Utc.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task AddAsync(ParsedMatchWardInfoModel model);

	/// <summary>
	/// Adds a list of parsed match records.
	/// </summary>
	/// <param name="list">IEnumerable of match records. Consists of (matchId, accountId, DateTime).</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task AddRangeAsync(IEnumerable<ParsedMatchWardInfoModel> list);

	/// <summary>
	/// Deletes the parsed match record.
	/// </summary>
	/// <param name="matchId">Match id to delete.</param>
	/// <param name="accountId">Account id to delete.</param>
	/// <returns></returns>
	Task RemoveAsync(long matchId, long accountId);

	/// <summary>
	/// Checks if the match is already parsed (meaning info about wards are added into other table).
	/// </summary>
	/// <param name="matchId">Match id to check.</param>
	/// <param name="accountId">Account id to check.</param>
	/// <returns>Task representing asynchronous operation. 
	/// Task result contains <b>true</b> is the match is parsed and info is added into other tables, otherwise <b>false</b>.</returns>
	Task<bool> IsMatchParsedAsync(long matchId, long accountId);

	/// <summary>
	/// Gets the parsed matches saved in storage for given account.
	/// </summary>
	/// <param name="accountId">Account id.</param>
	/// <returns>Task representing asynchronous operation.
	/// Task result contains <see cref="IEnumerable{T}"/> of <see cref="ParsedMatchWardInfoModel"/> objects.</returns>
	Task<IEnumerable<ParsedMatchWardInfoModel>> GetParsedMatchesForAccountAsync(long accountId);
}

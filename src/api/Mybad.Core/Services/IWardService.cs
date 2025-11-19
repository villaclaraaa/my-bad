using Mybad.Core.DomainModels;

namespace Mybad.Core.Services;

/// <summary>
/// Contract for ward-related operations with storage.
/// </summary>
public interface IWardService
{
	/// <summary>
	/// Adds a ward entry to storage.
	/// </summary>
	/// <param name="ward"><see cref="WardModel"/> instance to add.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task AddAsync(WardModel ward);

	Task AddRangeAsync(IEnumerable<WardModel> wards);

	/// <summary>
	/// Gets all wards for a specific match.
	/// </summary>
	/// <param name="matchId">Match id to get info.</param>
	/// <param name="accountId">Account id to get info.</param>
	/// <returns>Task representing asynchronous operation. 
	/// Task contains list of <see cref="WardModel"/> as <see cref="IEnumerable{T}"/>.</returns>
	Task<IEnumerable<WardModel>> GetAllByMatchAsync(long matchId, long accountId);

	/// <summary>
	/// Gets all wards for a specific account.
	/// </summary>
	/// <param name="accountId">Account id to get info.</param>
	/// <returns>Task representing asynchronous operation. 
	/// Task contains list of <see cref="WardModel"/> as <see cref="IEnumerable{T}"/>.</returns>
	Task<IEnumerable<WardModel>> GetAllForAccountAsync(long accountId);

	/// <summary>
	/// Deletes the wards for a specific account.
	/// </summary>
	/// <param name="accountId">Account id of user.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task DeleteAllForAccountAsync(long accountId);

	/// <summary>
	/// Deletes all wards from a specific match.
	/// </summary>
	/// <param name="matchId">Match id to delete.</param>
	/// <returns>Task representing asynchronous operation.</returns>
	Task DeleteAllFromMatchAsync(long matchId);
}

namespace Mybad.Core.Services;

/// <summary>
/// Contracts for tracking and filtering match identifiers that have already been checked for HeroMatchup.
/// </summary>
public interface ICheckedMatchesService
{
    /// <summary>
    /// Filters the specified list of match identifiers to exclude those that have already been checked.
    /// </summary>
    /// <param name="matches">A list of match IDs to be filtered. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// Result contains a list of match IDs that have not yet been checked.</returns>
    public Task<List<long>> FilterAlreadyCheckedMatches(List<long> matches);

    /// <summary>
    /// Adds the specified collection of checked match identifiers to the data store asynchronously.
    /// </summary>
    /// <param name="checkedMatches">A list of match IDs to be marked as checked. Cannot be null or contain duplicate values.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddCheckedMatches(List<long> checkedMatches, int patchId);

    /// <summary>
    /// Gets checked matches count.
    /// </summary>
    public long CheckedMatchesCount { get; }
}

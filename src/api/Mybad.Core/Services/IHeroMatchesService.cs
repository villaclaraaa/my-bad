using Mybad.Core.Providers.CoreHeroMatchupProvider;

namespace Mybad.Core.Services
{
    /// <summary>
    /// Provides methods for managing and retrieving hero match statistics.
    /// </summary>
    /// <remarks>This service allows updating hero match statistics in bulk and retrieving statistics for a
    /// specific hero.</remarks>
    public interface IHeroMatchesService
    {
        /// <summary>
        /// Updates the hero match statistics with the provided data.
        /// </summary>
        /// <param name="heroMatches">A dictionary where the key is the hero ID and the value is the corresponding game results statistics.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateHeroMatches(Dictionary<int, GamesResultsStat> heroMatches);

        /// <summary>
        /// Retrieves statistical data for matches played by a specific hero.
        /// </summary>
        /// <remarks>This method is asynchronous and should be awaited. The returned statistics include 
        /// aggregated data such as win rates, total matches played, and other relevant metrics.</remarks>
        /// <param name="id">The unique identifier of the hero whose match statistics are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a  <see
        /// cref="GamesResultsStat"/> object with the statistical data for the hero's matches.</returns>
        Task<GamesResultsStat> GetHeroMatchesStat(int id);
    }
}

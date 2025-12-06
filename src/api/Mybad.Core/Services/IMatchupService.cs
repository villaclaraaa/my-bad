using Mybad.Core.Providers.CoreHeroMatchupProvider;

namespace Mybad.Core.Services;

/// <summary>
/// Contracts for updating hero matchup statistics and calculating optimal hero matchups against enemies, 
/// with allies, or in combination.
/// </summary>
public interface IMatchupService
{
	/// <summary>
	/// Updates the matchup statistics for hero pairs based on the provided game results.
	/// </summary>
	/// <param name="heroStats">A dictionary containing matchup statistics, where each key is a tuple of hero IDs representing a hero pair, and
	/// each value is the corresponding game results statistic to update.</param>
	/// <param name="isEnemyMatchup">Indicates whether the matchup statistics should be updated for enemy hero pairs. Set to <see langword="true"/> to
	/// update enemy matchups; otherwise, updates ally matchups.</param>
	/// <returns>A task that represents the asynchronous update operation.</returns>
	Task UpdateMatchups(Dictionary<(int HeroId, int OtherHeroId), GamesResultsStat> heroStats, bool isEnemyMatchup);

	/// <summary>
	/// Calculates the best hero matchups against the specified enemy hero IDs.
	/// </summary>
	/// <param name="ids">A list of integer IDs representing the enemy heroes to evaluate matchups against.</param>
	/// <returns>A task that represents the asynchronous operation. 
	/// The task result contains a list of hero matchup models ranked by effectiveness against the specified enemies.</returns>
	Task<List<HeroMatchupModel>> CalcutaleBestMatchupVersusEnemies(List<int> ids);

	/// <summary>
	/// Calculates the optimal hero matchups when paired with the specified allies.
	/// </summary>
	/// <param name="ids">A list of hero IDs representing the allies to consider when determining the best matchups.</param>
	/// <returns>A task that represents the asynchronous operation. 
	/// The task result contains a list of hero matchup models representing the best matchups for the given allies.</returns>
	Task<List<HeroMatchupModel>> CalcutaleBestMatchupWithAllies(List<int> ids);

	/// <summary>
	/// Calculates the optimal hero matchups based on the specified enemy and ally hero IDs.
	/// </summary>
	/// <param name="enemyIds">A list of hero IDs representing the enemy team.</param>
	/// <param name="allyIds">A list of hero IDs representing the ally team.</param>
	/// <returns>A task that represents the asynchronous operation. 
	/// The task result contains a list of hero matchup models representing the best combined matchups for the given teams.</returns>
	Task<List<HeroMatchupModel>> CalcutaleBestMatchupCombined(List<int> enemyIds, List<int> allyIds);
}

using Microsoft.EntityFrameworkCore;
using Mybad.Core.Providers.CoreHeroMatchupProvider;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services;

public class MatchupService : IMatchupService
{
    private readonly ApplicationDbContext _dbContext;
    private const int _minGames = 10;
    public MatchupService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task UpdateMatchups(
        Dictionary<(int HeroId, int OtherHeroId), GamesResultsStat> heroStats,
        bool isEnemyMatchup)
    {
        var keys = heroStats.Keys.ToList();
        if (!keys.Any())
            return;

        var heroIds = keys.Select(k => k.HeroId).Distinct().ToList();
        var otherIds = keys.Select(k => k.OtherHeroId).Distinct().ToList();

        if (isEnemyMatchup)
        {
            var existingMatchups = await _dbContext.HeroMatchupEnemies
                .Where(hm => heroIds.Contains(hm.HeroId) && otherIds.Contains(hm.OtherHeroId))
                .ToListAsync();

            foreach (var kv in heroStats)
            {
                var key = kv.Key;
                var stat = kv.Value;

                var entity = existingMatchups
                    .FirstOrDefault(hm => hm.HeroId == key.HeroId && hm.OtherHeroId == key.OtherHeroId);

                if (entity != null)
                {
                    entity.Wins += stat.Wins;
                    entity.GamesPlayed += stat.GamesPlayed;
                }
                else
                {
                    _dbContext.HeroMatchupEnemies.Add(new HeroMatchupEnemyEntity
                    {
                        HeroId = key.HeroId,
                        OtherHeroId = key.OtherHeroId,
                        Wins = stat.Wins,
                        GamesPlayed = stat.GamesPlayed
                    });
                }
            }
        }
        else
        {
            var existingMatchups = await _dbContext.HeroMatchupAllies
                .Where(hm => heroIds.Contains(hm.HeroId) && otherIds.Contains(hm.OtherHeroId))
                .ToListAsync();

            foreach (var kv in heroStats)
            {
                var key = kv.Key;
                var stat = kv.Value;

                var entity = existingMatchups
                    .FirstOrDefault(hm => hm.HeroId == key.HeroId && hm.OtherHeroId == key.OtherHeroId);

                if (entity != null)
                {
                    entity.Wins += stat.Wins;
                    entity.GamesPlayed += stat.GamesPlayed;
                }
                else
                {
                    _dbContext.HeroMatchupAllies.Add(new HeroMatchupAllyEntity
                    {
                        HeroId = key.HeroId,
                        OtherHeroId = key.OtherHeroId,
                        Wins = stat.Wins,
                        GamesPlayed = stat.GamesPlayed
                    });
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupVersusEnemies(List<int> ids)
    {
        var matchupsEnemies = await _dbContext.HeroMatchupEnemies
            .Where(hm => ids.Contains(hm.HeroId) && hm.GamesPlayed >= _minGames)
            .ToListAsync();

        var matchesStats = await _dbContext.HeroesMatches.ToListAsync();

        var heroRatings = ComputeHeroRatings(matchupsEnemies, matchesStats, isEnemyData: true);

        var bestModels = ConvertMatchupDictionaryToModelsList(heroRatings);
        return bestModels;
    }

    /// <inheritdoc />
    public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupWithAllies(List<int> ids)
    {
        var matchupsAllies = await _dbContext.HeroMatchupAllies
           .Where(hm => ids.Contains(hm.HeroId) && hm.GamesPlayed >= _minGames)
           .ToListAsync();

        var matchesStats = await _dbContext.HeroesMatches.ToListAsync();

        var heroRatings = ComputeHeroRatings(matchupsAllies, matchesStats, isEnemyData: false);

        var bestModels = ConvertMatchupDictionaryToModelsList(heroRatings);
        return bestModels;
    }

    /// <inheritdoc />
    public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupCombined(List<int> enemyIds, List<int> allyIds)
    {
        var matchupsEnemies = await _dbContext.HeroMatchupEnemies
            .Where(hm => enemyIds.Contains(hm.HeroId) && hm.GamesPlayed >= _minGames)
            .Where(hm => !enemyIds.Contains(hm.OtherHeroId))
            .ToListAsync();

        var matchupsAllies = await _dbContext.HeroMatchupAllies
            .Where(hm => allyIds.Contains(hm.HeroId) && hm.GamesPlayed >= _minGames)
            .Where(hm => !allyIds.Contains(hm.OtherHeroId))
            .ToListAsync();

        var enemyMatchesStats = await _dbContext.HeroesMatches.ToListAsync();
        var allyMatchesStats = await _dbContext.HeroesMatches.ToListAsync();

        var enemyHeroRatings = ComputeHeroRatings(matchupsEnemies, enemyMatchesStats, isEnemyData: true);
        var allyHeroRatings = ComputeHeroRatings(matchupsAllies, allyMatchesStats, isEnemyData: false);

        var allHeroIds = enemyHeroRatings.Keys.Union(allyHeroRatings.Keys).ToHashSet();
        var combinedHeroRatings = new Dictionary<int, (double Rating, double Confidence)>();
        foreach (var heroId in allHeroIds)
        {
            var hasEnemyData = enemyHeroRatings.TryGetValue(heroId, out var enemyData);
            var hasAllyData = allyHeroRatings.TryGetValue(heroId, out var allyData);

            if (hasEnemyData && hasAllyData)
            {
                var combinedRating = 0.7 * enemyData.Rating + 0.3 * allyData.Rating;
                var combinedConfidence = (enemyData.Confidence + allyData.Confidence) / 2;

                combinedHeroRatings[heroId] = (combinedRating, combinedConfidence);
            }
            else if (hasEnemyData)
            {
                combinedHeroRatings[heroId] = (enemyData.Rating * 0.8, enemyData.Confidence);
            }
            else if (hasAllyData)
            {
                combinedHeroRatings[heroId] = (allyData.Rating * 0.8, allyData.Confidence);
            }
        }

        return ConvertMatchupDictionaryToModelsList(combinedHeroRatings);
    }

    private Dictionary<int, (double Rating, double Confidence)> ComputeHeroRatings<T, G>(List<T> matchups, List<G> matchesStats, bool isEnemyData = true)
        where T : HeroMatchupEntity where G : HeroMatchesEntity
    {
        var heroData = new Dictionary<int, List<(double WinRate, int Games)>>();

        // Step 1: Group matchup data by hero
        foreach (var m in matchups)
        {
            if (m.GamesPlayed == 0) continue; // Skip invalid data

            var winRate = (double)m.Wins / m.GamesPlayed;
            if (!heroData.ContainsKey(m.OtherHeroId))
                heroData[m.OtherHeroId] = new List<(double, int)>();

            heroData[m.OtherHeroId].Add((winRate, m.GamesPlayed));
        }

        var heroRatings = new Dictionary<int, (double Rating, double Confidence)>();

        // Step 2: Bayesian parameters
        const double priorStrength = 20;

        foreach (var kv in heroData)
        {
            var heroId = kv.Key;
            var matchupData = kv.Value;

            var heroGameStats = matchesStats.FirstOrDefault(h => h.HeroId == heroId);
            double globalWinrate = (double)heroGameStats!.TotalWins / heroGameStats.TotalGamesPlayed;

            // Step 3: Calculate total wins and games
            var totalWins = heroGameStats!.TotalWins;
            var totalGames = heroGameStats.TotalGamesPlayed;

            // Step 4: Bayesian smoothing
            var bayesianWinRate = (totalWins + priorStrength * globalWinrate) / (totalGames + priorStrength);

            // Step 5: Calculate effectiveness based on data type
            double effectivenessScore;
            if (isEnemyData)
            {
                // For enemies: lower enemy win rate = better for us
                effectivenessScore = globalWinrate - bayesianWinRate;
            }
            else
            {
                // For allies: higher ally win rate = better for us
                effectivenessScore = bayesianWinRate - globalWinrate;
            }

            // Step 6: Calculate confidence
            var p = bayesianWinRate;
            var n = totalGames + priorStrength;
            var z = 1.96;

            var confidenceInterval = z * Math.Sqrt((p * (1 - p)) / n);
            var confidence = Math.Max(0, 1.0 - confidenceInterval);

            // Step 7: Variance penalty
            var variance = matchupData.Count > 1
                ? matchupData.Select(x => Math.Pow(x.WinRate - bayesianWinRate, 2)).Average()
                : 0;
            var variancePenalty = Math.Max(0.7, 1.0 - variance * 2);

            // Step 8: Final rating
            var finalRating = effectivenessScore * confidence * variancePenalty * 100;

            heroRatings[heroId] = (finalRating, confidence);
        }

        return heroRatings;
    }

    private List<HeroMatchupModel> ConvertMatchupDictionaryToModelsList(Dictionary<int, (double Rating, double Confidence)> ratings)
    {
        var best = ratings.OrderByDescending(k => k.Value.Rating)
            .ThenByDescending(k => k.Value.Confidence)
            .Take(5).ToDictionary();
        var bestModels = new List<HeroMatchupModel>();
        foreach (var m in best)
        {
            bestModels.Add(new HeroMatchupModel() { HeroId = m.Key, Rating = m.Value.Rating });
        }
        return bestModels;
    }
}

using Microsoft.EntityFrameworkCore;
using Mybad.Core.Providers.CoreHeroMatchupProvider;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services
{
    public class MatchupService : IMatchupService
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchupService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupVersusEnemies(List<int> ids)
        {
            var matchups = await _dbContext.HeroMatchupEnemies
                .Where(hm => ids.Contains(hm.HeroId))
                .OrderByDescending(x => (x.Wins / x.GamesPlayed)) //THIS LINES CAN BE REMOVED
                .Take(20)                                         //TEST IF THERE ARE SOME SIGNIFICANT CHANGES IN OUTPUT
                .ToListAsync();

            var heroRatings = ComputeHeroRatings(matchups);

            var bestModels = ConvertMatchupDictionaryToModelsList(heroRatings);
            return bestModels;
        }

        public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupWithAllies(List<int> ids)
        {
            var matchups = await _dbContext.HeroMatchupAllies
                .Where(hm => ids.Contains(hm.HeroId))
                .OrderByDescending(x => (x.Wins / x.GamesPlayed)) //THIS LINES CAN BE REMOVED
                .Take(20)                                         //TEST IF THERE ARE SOME SIGNIFICANT CHANGES IN OUTPUT
                .ToListAsync();

            var heroRatings = ComputeHeroRatings(matchups);

            var bestModels = ConvertMatchupDictionaryToModelsList(heroRatings);
            return bestModels;
        }
        public async Task<List<HeroMatchupModel>> CalcutaleBestMatchupCombined(List<int> enemyIds, List<int> allyIds)
        {
            var matchupsEnemies = await _dbContext.HeroMatchupEnemies
                .Where(hm => enemyIds.Contains(hm.HeroId))
                .OrderByDescending(x => (x.Wins / x.GamesPlayed)) //THIS LINES CAN BE REMOVED
                .Take(20)                                         //TEST IF THERE ARE SOME SIGNIFICANT CHANGES IN OUTPUT
                .ToListAsync();

            var matchupsAllies = await _dbContext.HeroMatchupAllies
                .Where(hm => allyIds.Contains(hm.HeroId))
                .OrderByDescending(x => (x.Wins / x.GamesPlayed)) //THIS LINES CAN BE REMOVED
                .Take(20)                                         //TEST IF THERE ARE SOME SIGNIFICANT CHANGES IN OUTPUT
                .ToListAsync();

            var enemyHeroRatings = ComputeHeroRatings(matchupsEnemies);
            var allyHeroRatings = ComputeHeroRatings(matchupsAllies);

            var combinedHeroRatings = new Dictionary<int, double>();
            foreach (var enemyId in enemyHeroRatings.Keys)
            {
                if (allyHeroRatings.ContainsKey(enemyId))
                {
                    combinedHeroRatings.Add(enemyId, enemyHeroRatings[enemyId] + 0.5 * allyHeroRatings[enemyId]);
                }
                else
                {
                    combinedHeroRatings.Add(enemyId, enemyHeroRatings[enemyId]);
                }
            }

            var bestModels = ConvertMatchupDictionaryToModelsList(combinedHeroRatings);
            return bestModels;

        }

        private Dictionary<int, double> ComputeHeroRatings<T>(List<T> matchups) where T : HeroMatchupEntity
        {
            var heroRatings = new Dictionary<int, double>();
            foreach (var m in matchups)
            {
                if (!heroRatings.ContainsKey(m.OtherHeroId))
                {
                    heroRatings.Add(m.OtherHeroId, (double)100 - (double)m.Wins / m.GamesPlayed);
                }
                else
                {
                    heroRatings[m.OtherHeroId] += (double)100 - (double)m.Wins / m.GamesPlayed;
                }
            }
            return heroRatings;
        }

        private List<HeroMatchupModel> ConvertMatchupDictionaryToModelsList(Dictionary<int, double> ratings)
        {
            var best = ratings.OrderByDescending(k => k.Value).Take(5).ToDictionary();
            var bestModels = new List<HeroMatchupModel>();
            foreach (var m in best)
            {
                bestModels.Add(new HeroMatchupModel() { HeroId = m.Key, Rating = m.Value });
            }
            return bestModels;
        }
    }
}

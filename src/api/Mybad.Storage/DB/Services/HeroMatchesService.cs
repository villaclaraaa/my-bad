using Microsoft.EntityFrameworkCore;
using Mybad.Core.Providers.CoreHeroMatchupProvider;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services
{
    public class HeroMatchesService : IHeroMatchesService
    {
        private readonly ApplicationDbContext _dbContext;
        public HeroMatchesService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<GamesResultsStat> GetHeroMatchesStat(int id)
        {
            GamesResultsStat result = new GamesResultsStat(0, 0);

            var heroStats = await _dbContext.HeroesMatches.FindAsync(id);

            if (heroStats != null)
            {
                result.Wins = heroStats.TotalWins;
                result.GamesPlayed = heroStats.TotalGamesPlayed;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task UpdateHeroMatches(Dictionary<int, GamesResultsStat> heroMatches)
        {
            if (heroMatches == null || !heroMatches.Any())
                return;

            var heroIds = heroMatches.Keys.ToList();

            var existingHeroes = await _dbContext.HeroesMatches
                .Where(h => heroIds.Contains(h.HeroId))
                .ToListAsync();

            foreach (var kv in heroMatches)
            {
                var heroId = kv.Key;
                var stats = kv.Value;

                var existingHero = existingHeroes.FirstOrDefault(h => h.HeroId == heroId);

                if (existingHero != null)
                {
                    existingHero.TotalWins += stats.Wins;
                    existingHero.TotalGamesPlayed += stats.GamesPlayed;
                }
                else
                {
                    Console.WriteLine(heroId);
                    _dbContext.HeroesMatches.Add(new HeroMatchesEntity
                    {
                        HeroId = heroId,
                        TotalWins = stats.Wins,
                        TotalGamesPlayed = stats.GamesPlayed
                    });

                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

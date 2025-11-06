using Mybad.Core.Models;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using System.Text.Json;

namespace Mybad.Core.Providers
{
    public class CoreHeroMatchupProvider : IInfoProvider<HeroMatchupRequest, HeroMatchupResponse>
    {
        private static async Task<(List<int> calcualtedBestHeroes, List<int> bestVersus, List<int> bestWith)> FindBestHeroVsEnemyWithAlly(List<int> enemyIds, List<int> allyIds)
        {
            string enemyMatchupsFilePath = @"C:\Users\Andrew\Desktop\enemyMatchups.json";
            string allyMatchupsFilePath = @"C:\Users\Andrew\Desktop\allyMatchups.json";

            using StreamReader srEnemy = new StreamReader(enemyMatchupsFilePath);
            var jsonMatchups = srEnemy.ReadToEnd();
            var enemyHeroMatchups = new Dictionary<int, Dictionary<int, GamesResultsStat>>();
            enemyHeroMatchups = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GamesResultsStat>>>(jsonMatchups); //you need this

            using var srAlly = new StreamReader(allyMatchupsFilePath);
            jsonMatchups = srAlly.ReadToEnd();
            var allyHeroMatchups = new Dictionary<int, Dictionary<int, GamesResultsStat>>();
            allyHeroMatchups = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GamesResultsStat>>>(jsonMatchups); // you also need this

            var heroRatingVsEnemy = new Dictionary<int, double>();
            var heroRatingWithAlly = new Dictionary<int, double>();
            var heroRatingTotal = new Dictionary<int, double>();

            //CODE BELOW HAS TO BE REWRITTEN IN MORE EFFICIENT WAY, I JUST CAN'T COME UP WITH IDEA HOW
            //REWRITE PLZ IN AFTER TESTS HOW TO PROPERLY IMPLEMENT ALLY STATS IN TOTAL STATS
            foreach (var heroId in enemyIds)
            {
                var matchups = enemyHeroMatchups[heroId];
                foreach (var id in matchups.Keys)
                {
                    if (enemyIds.Contains(id))
                        continue;
                    var stats = matchups[id];
                    if (!heroRatingVsEnemy.ContainsKey(id))
                    {
                        heroRatingVsEnemy.Add(id, (double)100 - (double)stats.Wins / stats.GamesPlayed);
                    }
                    else
                    {
                        heroRatingVsEnemy[id] += (double)100 - (double)stats.Wins / stats.GamesPlayed;
                    }
                }

            }
            foreach (var heroId in allyIds)
            {
                var matchups = allyHeroMatchups[heroId];
                foreach (var id in matchups.Keys)
                {
                    if (enemyIds.Contains(id))
                        continue;
                    var stats = matchups[id];
                    if (!heroRatingWithAlly.ContainsKey(id))
                    {
                        heroRatingWithAlly.Add(id, (double)stats.Wins / stats.GamesPlayed);
                    }
                    else
                    {
                        heroRatingWithAlly[id] += (double)stats.Wins / stats.GamesPlayed;
                    }
                }
            }

            heroRatingVsEnemy = heroRatingVsEnemy.OrderByDescending(r => r.Value).ToDictionary();
            heroRatingWithAlly = heroRatingWithAlly.OrderByDescending(r => r.Value).ToDictionary();

            foreach (var heroId in heroRatingVsEnemy.Keys)
            {
                heroRatingTotal.Add(heroId, heroRatingVsEnemy[heroId] + (0.5 * heroRatingWithAlly[heroId]));
            }

            heroRatingTotal = heroRatingTotal.OrderByDescending(r => r.Value).ToDictionary();

            return (heroRatingTotal.Keys.Take(5).ToList(),
                    heroRatingVsEnemy.Keys.Take(5).ToList(),
                    heroRatingWithAlly.Keys.Take(5).ToList());
        }

        public async Task<HeroMatchupResponse> GetInfo(HeroMatchupRequest request)
        {
            var r = await FindBestHeroVsEnemyWithAlly(request.EnemyIds, request.AllyIds);
            var response = new HeroMatchupResponse() { BestCalculated = r.calcualtedBestHeroes, BestVersus = r.bestVersus, BestWith = r.bestWith };
        }
    }

}

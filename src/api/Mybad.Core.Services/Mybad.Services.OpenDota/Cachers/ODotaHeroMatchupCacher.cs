using Microsoft.Extensions.DependencyInjection;
using Mybad.Core.Services;
using Mybad.Services.OpenDota.ApiResponseModels;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using GamesResultsStat = Mybad.Core.Providers.CoreHeroMatchupProvider.GamesResultsStat;

namespace Mybad.Services.OpenDota.Cachers
{
    public class ODotaHeroMatchupCacher
    {
        private readonly IHttpClientFactory _factory;
        private readonly ICheckedMatchesService _checkedMatchesService;
        private readonly IHeroMatchesService _heroMatchesService;
        private readonly IServiceProvider _sp;

        public ODotaHeroMatchupCacher(IHttpClientFactory factory, ICheckedMatchesService checkedMatchesService, IHeroMatchesService heroMatchesService, IServiceProvider sp)
        {
            _factory = factory;
            _checkedMatchesService = checkedMatchesService;
            _heroMatchesService = heroMatchesService;
            _sp = sp;
        }

        public long CachedMatchesCount => _checkedMatchesService.CheckedMatchesCount;

        public async Task UpdateHeroMatchupsDatabase(int minRank)
        {
            var http = _factory.CreateClient("ODota");
            var heroStatsEnemy = new Dictionary<(int HeroId, int EnemyHeroId), GamesResultsStat>();
            var heroStatsAlly = new Dictionary<(int HeroId, int AllyHeroId), GamesResultsStat>();

            var heroMatchesStats = new Dictionary<int, GamesResultsStat>(); //stores heroID and its winrate

            int patchId = 59; //dafault value, 7.40 patch

            for (int i = 0; i < 1; i++)
            {
                var response = await http.GetFromJsonAsync<List<PublicMatchModel>>($"publicMatches?min_rank={minRank}");

                if (response == null)
                {
                    throw new InvalidOperationException();
                }
                var uncheckedMatches = await _checkedMatchesService.FilterAlreadyCheckedMatches(response.Select(r => r.MatchId).ToList());
                var uncheckedSet = new HashSet<long>(uncheckedMatches);

                var firstGameId = response.FirstOrDefault()?.MatchId;
                var jsonNode = await http.GetFromJsonAsync<JsonNode>($"matches/{firstGameId}");

                if (jsonNode != null)
                {
                    patchId = jsonNode["patch"].GetValue<int>();
                }

                foreach (var game in response.Where(g => uncheckedSet.Contains(g.MatchId)))
                {
                    game.SortTeams();

                    var winners = game.RadiantWin ? game.RadiantTeam : game.DireTeam;
                    var losers = game.RadiantWin ? game.DireTeam : game.RadiantTeam;
                    if (winners.Contains(0) || losers.Contains(0))
                    {
                        continue;
                    }

                    //go through all winners
                    foreach (var heroW in winners)
                    {
                        //ally
                        foreach (var allyW in winners)
                        {
                            if (heroW >= allyW) continue;
                            UpdateStats(heroStatsAlly, heroW, allyW, true);
                            UpdateStats(heroStatsAlly, allyW, heroW, true);
                        }

                        //winner vs loser
                        foreach (var heroL in losers)
                        {
                            UpdateStats(heroStatsEnemy, heroW, heroL, true);
                            UpdateStats(heroStatsEnemy, heroL, heroW, false);
                        }

                        UpdateHeroMatchesStats(heroMatchesStats, heroW, true);
                    }

                    //losers with losers
                    //don't need to make a loser vs winner because made it above
                    foreach (var heroL in losers)
                    {
                        foreach (var allyL in losers)
                        {
                            if (heroL >= allyL) continue;
                            UpdateStats(heroStatsAlly, heroL, allyL, false);
                            UpdateStats(heroStatsAlly, allyL, heroL, false);
                        }

                        UpdateHeroMatchesStats(heroMatchesStats, heroL, false);
                    }
                }
                Console.WriteLine($"Finished parsing game {i}");

                await Task.Delay(1000);

                await _checkedMatchesService.AddCheckedMatches(uncheckedMatches, patchId);
            }

            /* This is used to have 2 different matchupServices that contains two different DbContexts
			 * as inside UpdateMatchups() method we have condition on which dbcontext we do work.
			 */
            using var scope1 = _sp.CreateScope();
            var service1 = scope1.ServiceProvider.GetRequiredService<IMatchupService>();
            await service1.UpdateMatchups(heroStatsEnemy, isEnemyMatchup: true, patchId: patchId);

            using var scope2 = _sp.CreateScope();
            var service2 = scope2.ServiceProvider.GetRequiredService<IMatchupService>();
            await service2.UpdateMatchups(heroStatsAlly, isEnemyMatchup: false, patchId: patchId);

            await _heroMatchesService.UpdateHeroMatches(heroMatchesStats, patchId);
        }
        private static void UpdateStats(Dictionary<(int, int), GamesResultsStat> dict, int heroA, int heroB, bool didHeroAWin)
        {
            if (!dict.ContainsKey((heroA, heroB)))
            {
                dict.Add((heroA, heroB), new GamesResultsStat(0, 0));
            }

            if (didHeroAWin)
            {
                dict[(heroA, heroB)].Wins++;
            }
            dict[(heroA, heroB)].GamesPlayed++;
        }

        private static void UpdateHeroMatchesStats(Dictionary<int, GamesResultsStat> dict, int heroId, bool wonGame)
        {
            if (!dict.ContainsKey(heroId))
            {
                dict.Add(heroId, new GamesResultsStat(0, 0));
            }

            if (wonGame)
            {
                dict[heroId].Wins++;
            }

            dict[heroId].GamesPlayed++;
        }
    }
}

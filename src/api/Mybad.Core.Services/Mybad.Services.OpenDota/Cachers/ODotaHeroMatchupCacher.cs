using Microsoft.Extensions.DependencyInjection;
using Mybad.Core.Services;
using Mybad.Services.OpenDota.ApiResponseModels;
using System.Net.Http.Json;
using GamesResultsStat = Mybad.Core.Providers.CoreHeroMatchupProvider.GamesResultsStat;

namespace Mybad.Services.OpenDota.Cachers
{
	public class ODotaHeroMatchupCacher
	{
		private readonly IHttpClientFactory _factory;
		private readonly ICheckedMatchesService _checkedMatchesService;
		private readonly IServiceProvider _sp;

		public ODotaHeroMatchupCacher(IHttpClientFactory factory, ICheckedMatchesService checkedMatchesService, IServiceProvider sp)
		{
			_factory = factory;
			_checkedMatchesService = checkedMatchesService;
			_sp = sp;
		}
		public async Task UpdateHeroMatchupsDatabase(int minRank)
		{
			var http = _factory.CreateClient("ODota");
			var heroStatsEnemy = new Dictionary<(int HeroId, int EnemyHeroId), GamesResultsStat>();
			var heroStatsAlly = new Dictionary<(int HeroId, int AllyHeroId), GamesResultsStat>();

			for (int i = 0; i < 1; i++)
			{
				var response = await http.GetFromJsonAsync<List<PublicMatchModel>>($"publicMatches?min_rank={minRank}");

				if (response == null)
				{
					throw new InvalidOperationException();
				}
				var uncheckedMatches = await _checkedMatchesService.FilterAlreadyCheckedMatches(response.Select(r => r.MatchId).ToList());
				var uncheckedSet = new HashSet<long>(uncheckedMatches);

				Console.WriteLine("got response");
				foreach (var game in response.Where(g => uncheckedSet.Contains(g.MatchId)))
				{
					game.SortTeams();

					var winners = game.RadiantWin ? game.RadiantTeam : game.DireTeam;
					var losers = game.RadiantWin ? game.DireTeam : game.RadiantTeam;

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
					}
				}
				Console.WriteLine($"Finished parsing game {i}");

				await Task.Delay(1000);

				await _checkedMatchesService.AddCheckedMatches(uncheckedMatches);
			}

			/* This is used to have 2 different matchupServices that contains two different DbContexts
			 * as inside UpdateMatchups() method we have condition on which dbcontext we do work.
			 */
			using var scope1 = _sp.CreateScope();
			var service1 = scope1.ServiceProvider.GetRequiredService<IMatchupService>();
			await service1.UpdateMatchups(heroStatsEnemy, true);

			using var scope2 = _sp.CreateScope();
			var service2 = scope2.ServiceProvider.GetRequiredService<IMatchupService>();
			await service2.UpdateMatchups(heroStatsAlly, false);

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
	}

}

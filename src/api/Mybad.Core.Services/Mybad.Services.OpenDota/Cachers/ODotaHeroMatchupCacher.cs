using Mybad.Services.OpenDota.ApiResponseModels;
using System.Net.Http.Json;
using System.Text.Json;
using GamesResultsStat = Mybad.Core.Providers.CoreHeroMatchupProvider.GamesResultsStat;

namespace Mybad.Services.OpenDota.Cachers
{
    public class ODotaHeroMatchupCacher
    {
        private static string _urlPath = "https://api.opendota.com/api/";

        public async Task CachePublicMatchesInfo(int minRank)
        {
            using var http = new HttpClient();
            var heroStatsEnemy = new Dictionary<int, Dictionary<int, GamesResultsStat>>(); // <heroId, <enemyHeroId, (gamesPlayed, gamesWon)>> 
            var heroStatsAlly = new Dictionary<int, Dictionary<int, GamesResultsStat>>(); // <heroId, <enemyHeroId, (gamesPlayed, gamesWon)>> 

            string parsedMatchesPath = @"C:\Users\Andrew\Desktop\parsedMatchesIds.json";

            using StreamReader srMatches = new StreamReader(parsedMatchesPath);
            var jsonString = srMatches.ReadToEnd();
            var matchesId = JsonSerializer.Deserialize<HashSet<long>>(jsonString); //store checked matches id, so that you accidentaly dont check the same match more than once
            srMatches.Close();

            for (int i = 0; i < 1; i++)
            {
                var response = await http.GetFromJsonAsync<List<PublicMatchModel>>(_urlPath + $"publicMatches?min_rank={minRank}");

                if (response == null)
                {
                    throw new InvalidOperationException();
                }

                Console.WriteLine("got response");
                foreach (var game in response)
                {
                    if (matchesId.Contains(game.MatchId))
                        continue;
                    matchesId.Add(game.MatchId);
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
            }

            string enemyMatchupPath = @"C:\Users\Andrew\Desktop\enemyMatchups.json";
            string allyMatchupPath = @"C:\Users\Andrew\Desktop\allyMatchups.json";

            using StreamReader srEnemy = new StreamReader(enemyMatchupPath);
            jsonString = srEnemy.ReadToEnd();

            var storedStatsEnemy = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GamesResultsStat>>>(jsonString);
            srEnemy.Close();

            using StreamReader srAlly = new StreamReader(allyMatchupPath);
            jsonString = srAlly.ReadToEnd();
            var storedStatsAlly = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GamesResultsStat>>>(jsonString);
            srAlly.Close();
            //update the stats with new info
            foreach (var heroId in heroStatsEnemy.Keys)
            {
                foreach (var enemyId in heroStatsEnemy[heroId].Keys)
                {
                    storedStatsEnemy[heroId][enemyId].GamesPlayed += heroStatsEnemy[heroId][enemyId].GamesPlayed;
                    storedStatsEnemy[heroId][enemyId].Wins += heroStatsEnemy[heroId][enemyId].Wins;
                }
            }

            foreach (var heroId in heroStatsAlly.Keys)
            {
                foreach (var allyId in heroStatsAlly[heroId].Keys)
                {
                    storedStatsAlly[heroId][allyId].GamesPlayed += heroStatsAlly[heroId][allyId].GamesPlayed;
                    storedStatsAlly[heroId][allyId].Wins += heroStatsAlly[heroId][allyId].Wins;
                }
            }

            Console.WriteLine("Serializing");
            var options = new JsonSerializerOptions { WriteIndented = true };
            string finalJson = JsonSerializer.Serialize(storedStatsEnemy, options);
            await File.WriteAllTextAsync(enemyMatchupPath, finalJson);
            finalJson = JsonSerializer.Serialize(storedStatsAlly, options);
            await File.WriteAllTextAsync(allyMatchupPath, finalJson);

            finalJson = JsonSerializer.Serialize(matchesId, options);
            await File.WriteAllTextAsync(parsedMatchesPath, finalJson);

            Console.WriteLine("Finished serializing");

        }
        private static void UpdateStats(Dictionary<int, Dictionary<int, GamesResultsStat>> dict, int heroA, int heroB, bool didHeroAWin)
        {
            if (!dict.ContainsKey(heroA))
            {
                dict[heroA] = new Dictionary<int, GamesResultsStat>();
            }

            if (!dict[heroA].ContainsKey(heroB))
            {
                dict[heroA][heroB] = new GamesResultsStat(0, 0);
            }

            var gameResultsStats = dict[heroA][heroB];

            gameResultsStats.GamesPlayed++;
            if (didHeroAWin)
            {
                gameResultsStats.Wins++;
            }

            dict[heroA][heroB] = gameResultsStats;
        }
    }

}

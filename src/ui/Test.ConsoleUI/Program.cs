Console.WriteLine("1");

//IRequestService service = new OpendotaProvider();
//BaseRequest req = new WardLogRequest(136996088);

//var a = await service.GetData(req);
//WardsLogMatchResponse b = (WardsLogMatchResponse)a;
//foreach (var item in b.ObserverWardsLog)
//{
//	Console.WriteLine(item.X + " " + item.Y + "-" + item.TimeLived + " " + item.Amount);
//}
//foreach (var item in b.SentryWardsLog)
//{
//	Console.WriteLine(item.X + " " + item.Y + "-" + item.TimeLived + " " + item.Amount);
//}
//Console.WriteLine("END");


//var req = new WardMapRequest(136996088);

//IInfoProvider<WardMapRequest, WardsMapPlacementResponse> provider = new ODotaWardPlacementMapProvider();
//var responce = await provider.GetInfo(req);

//var response = await provider.GetInfo(req);

//foreach (var item in response.HeroMatchups)
//{
//    Console.WriteLine(item.HeroName + " " + item.WinrateAgainst + "%");
//}

//await ODotaHeroMatchupProvider.CacheHeroMatchupInfo();

//List<int> heroesId = new List<int> { 7, 128, 101, 99 };

//var bestHeroes = await HeroMatchupService.CalculateBestHeroesAgainstEnemyPick(heroesId);
//foreach (var hero in bestHeroes)
//{
//    Console.WriteLine((HeroesEnum)hero);
//}


//empty data filling
//string enemyMatchupsFilePath = @"C:\Users\Andrew\Desktop\enemyMatchups.json";
//string allyMatchupsFilePath = @"C:\Users\Andrew\Desktop\allyMatchups.json";

//var emptyData = new Dictionary<int, Dictionary<int, GamesResultsStat>>();
//Array heroValues = Enum.GetValues(typeof(HeroesEnum));
//foreach (int heroId in heroValues)
//{
//    emptyData.Add(heroId, new Dictionary<int, GamesResultsStat>());
//    foreach (int heroIdInner in heroValues)
//    {
//        if (heroId == heroIdInner) continue;

//        emptyData[heroId].Add(heroIdInner, new GamesResultsStat(0, 0));
//    }
//}

//var options = new JsonSerializerOptions { WriteIndented = true };
//string finalJson = JsonSerializer.Serialize(emptyData, options);
//await File.WriteAllTextAsync(enemyMatchupsFilePath, finalJson);
//await File.WriteAllTextAsync(allyMatchupsFilePath, finalJson);

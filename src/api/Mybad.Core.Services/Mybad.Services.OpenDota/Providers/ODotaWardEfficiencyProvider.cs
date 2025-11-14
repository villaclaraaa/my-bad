using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;
using Mybad.Services.OpenDota.ApiResponseReaders;
using System.Text.Json;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardEfficiencyProvider : IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>
{
	public async Task<WardsEfficiencyResponse> GetInfoAsync(WardsEfficiencyRequest request)
	{
		return (WardsEfficiencyResponse)await GetWardsLogInfo(request);
	}

	private static string _urlPath = "https://api.opendota.com/api/";

	/*
	 * Flow
	 * 1. Get player Id 
	 * 2. Get last {amount} matches ID for the player
	 * 3. On every match check if the match is parsed
	 * 4. If yes - then get wards info for match
	 * 4.1. if no - ask to parse and save the state somewhere
	 * 5. Wait for all matches to be checked 
	 * 6. return
	 */
	private async Task<BaseResponse> GetWardsLogInfo(WardsEfficiencyRequest request)
	{
		ArgumentNullException.ThrowIfNull(request.AccountId);

		//using var http = new HttpClient();
		//var response = await http.GetFromJsonAsync<List<RecentMatch>>(_urlPath + $"players/136996088/recentMatches");

		//if (response == null)
		//{
		//	throw new InvalidOperationException();
		//}
		//var reader = new WardsPlacementMapReader();

		// TODO - Check for database entries of matches

		var di = Directory.CreateDirectory("Data");
		//var tasks = response.Where(m => m.Lane != null)
		//	.Select(async match =>
		//	{
		//		var r = await http.GetFromJsonAsync<MatchWardLogInfo>(_urlPath + $"matches/{match.MatchId}");

		//		if (r != null)
		//		{
		//			var json = JsonSerializer.Serialize(r, new JsonSerializerOptions { WriteIndented = true });
		//			var filePath = Path.Combine("Data", $"{match.MatchId}.txt");
		//			await File.WriteAllTextAsync(filePath, json);
		//			return r;
		//		}

		//		return null;
		//	})
		//	.ToList();

		//var result = await Task.WhenAll(tasks);

		//var tasks = new List<Task>();
		//foreach (var match in response.Matches)
		//{
		//	if (match.Lane != null)
		//	{
		//		var task = Task.Run(async () =>
		//		{
		//			var r = await http.GetFromJsonAsync<MatchWardLogInfo>(_urlPath + $"matches/{match.MatchId}");
		//		}).ContinueWith(t =>
		//		{
		//			if (response == null)
		//			{
		//				throw new InvalidOperationException();
		//			}
		//		});
		//		tasks.Add(task);
		//	}
		//}


		/* Testing only single match from local storage
		 * Code above will probably be used later. 
		 */
		try
		{
			using var sr = new StreamReader(Path.Combine(di.FullName, "8522407623.txt"));
			var json = await sr.ReadToEndAsync();
			var apiResponse = JsonSerializer.Deserialize<MatchWardLogInfo>(json);

			var reader = new WardsPlacementMapReader();
			var obses = reader.ConvertWardsLogMatch(apiResponse!, request.AccountId).obses;

			if (!obses.Any())
			{
				throw new InvalidOperationException();
			}

			/*
			 * Finding multiple occurring of the approx. same wards
			 * And returning their average position
			 */
			int deviation = 2;
			var groups = new List<List<WardLog>>();

			foreach (var obs in obses)
			{
				// Find if point is near an existing group
				var existingGroup = groups.FirstOrDefault(g =>
					g.Any(q => Math.Abs(q.X - obs.X) <= deviation && Math.Abs(q.Y - obs.Y) <= deviation));

				if (existingGroup != null)
				{
					existingGroup.Add(obs);
				}
				else
				{
					groups.Add([obs]);
				}
			}

			// build final list with averaged points
			var result = groups.Select(g =>
			{
				double avgX = g.Average(p => p.X);
				double avgY = g.Average(p => p.Y);
				double avgTime = g.Average(p => p.TimeLived);
				/* 
				* Now the calcucating efficiency score
				*/
				return new WardEfficiency
				{
					X = (int)Math.Round(avgX),
					Y = (int)Math.Round(avgY),
					Amount = g.Count,
					AverageTimeLived = (int)Math.Round(avgTime),
					EfficiencyScore = (float)Math.Round(avgTime / 360, 1),
				};
			}).ToList();


			var response = new WardsEfficiencyResponse
			{
				Id = 1,
				ObserverWards = result
			};
			return response;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}
	}
}

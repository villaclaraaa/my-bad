using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Test.ConsoleUI;

public class OpenDota
{

	public async Task Do()
	{
		using var http = new HttpClient();
		var response = await http.GetFromJsonAsync<HeroResponse[]>("https://api.opendota.com/api/heroes/1/matchups");

		foreach (var r in response)
		{
			Console.WriteLine($"{r.HeroName} - Games {r.GamesPlayed}, Wins - {r.Percentage}%");
		}
	}

	public async Task DoMatch()
	{
		using var http = new HttpClient();
		var response = await http.GetFromJsonAsync<MatchInfo>("https://api.opendota.com/api/matches/8519566987");

		foreach (var p in response.Players)
		{
			Console.WriteLine($"Info for player slot - {p.Slot}");
			foreach (var o in p.ObsLeftLogs)
			{
				Console.WriteLine(o.Time);
			}
		}
	}


	public class HeroResponse
	{
		[JsonPropertyName("hero_id")]
		public int HeroAgainstId { get; set; }

		[JsonPropertyName("games_played")]
		public int GamesPlayed { get; set; }

		[JsonPropertyName("wins")]
		public int Wins { get; set; }

		public int Percentage => (int)((float)Wins / (float)GamesPlayed * 100);

		public HeroesEnum HeroName => (HeroesEnum)HeroAgainstId;
	}
}
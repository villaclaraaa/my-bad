using System.Text.Json.Serialization;

namespace Mybad.Core.Providers.CoreHeroMatchupProvider
{
	public class GamesResultsStat
	{
		[JsonPropertyName("wins")]
		public int Wins { get; set; }

		[JsonPropertyName("games_played")]
		public int GamesPlayed { get; set; }

		public GamesResultsStat(int wins, int gamesPlayed)
		{
			Wins = wins; GamesPlayed = gamesPlayed;
		}
	}
}

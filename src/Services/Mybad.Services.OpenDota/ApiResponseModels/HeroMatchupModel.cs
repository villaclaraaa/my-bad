using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels
{
    internal class HeroMatchupModel
    {
        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }

        [JsonPropertyName("games_played")]
        public int GamesPlayed { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }
    }
}

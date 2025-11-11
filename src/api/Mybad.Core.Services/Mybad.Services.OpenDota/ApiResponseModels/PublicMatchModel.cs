using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels
{
    public class PublicMatchModel
    {
        [JsonPropertyName("match_id")]
        public long MatchId { get; set; }

        [JsonPropertyName("radiant_win")]
        public bool RadiantWin { get; set; }

        [JsonPropertyName("radiant_team")]
        public List<int> RadiantTeam { get; set; } = new List<int>();

        [JsonPropertyName("dire_team")]
        public List<int> DireTeam { get; set; } = new List<int>();

        public void SortTeams()
        {
            RadiantTeam.Sort();
            DireTeam.Sort();
        }
    }
}

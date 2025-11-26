using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses
{
    public class HeroMatchupResponse : BaseResponse
    {
        public List<HeroMatchup> Matchup { get; set; } = null!;

    }
}

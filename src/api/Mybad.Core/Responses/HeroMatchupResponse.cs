using Mybad.Core.Responses.Entries;

namespace Mybad.Core.Responses
{
    public class HeroMatchupResponse : BaseResponse
    {
        public List<HeroMatchup> BestVersus { get; set; } = null!;
        public List<HeroMatchup> BestWith { get; set; } = null!;
        public List<HeroMatchup> BestCalculated { get; set; } = null!;

    }
}

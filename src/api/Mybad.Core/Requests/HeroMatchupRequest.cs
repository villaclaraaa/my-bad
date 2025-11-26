namespace Mybad.Core.Requests
{
    public class HeroMatchupRequest : BaseRequest
    {
        public List<int>? EnemyIds { get; set; }
        public List<int>? AllyIds { get; set; }
    }
}

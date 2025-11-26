namespace Mybad.Core.Services
{
    public interface ICheckedMatchesService
    {
        public Task<List<long>> FilterAlreadyCheckedMatches(List<long> matches);

        public Task AddCheckedMatches(List<long> checkedMatches);
    }
}

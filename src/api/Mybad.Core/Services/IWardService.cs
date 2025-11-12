using Mybad.Core.DomainModels;

namespace Mybad.Core.Services;

public interface IWardService
{
	Task AddAsync(WardModel ward);

	Task<IEnumerable<WardModel>> GetAllByMatchAsync(long matchId);

	Task<IEnumerable<WardModel>> GetAllForAccountAsync(long accountId);

	Task DeleteAllForAccountAsync(long accountId);

	Task DeleteAllFromMatchAsync(long matchId);

}

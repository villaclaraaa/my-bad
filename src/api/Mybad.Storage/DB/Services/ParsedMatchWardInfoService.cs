using Microsoft.EntityFrameworkCore;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services;

/// <summary>
/// Defines paresed match info related operations with storage using EFCore.
/// </summary>
public class ParsedMatchWardInfoService : IParsedMatchWardInfoService
{
	private readonly ApplicationDbContext _dbContext;

	public ParsedMatchWardInfoService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public async Task AddAsync(long matchId, long accountId, DateTime playedAtDateUtc)
	{
		await _dbContext.ParsedMatchWardInfos.AddAsync(new ParsedMatchWardInfo
		{
			MatchId = matchId,
			AccountId = accountId,
			PlayedAtDateUtc = playedAtDateUtc
		});
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task AddRangeAsync(IEnumerable<(long matchId, long accountId, DateTime playedAtDateUtc)> list)
	{
		await _dbContext.AddRangeAsync(list.Select(x => new ParsedMatchWardInfo
		{
			MatchId = x.matchId,
			AccountId = x.accountId,
			PlayedAtDateUtc = x.playedAtDateUtc
		}));
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<IEnumerable<long>> GetParsedMatchesForAccountAsync(long accountId) =>
		await _dbContext.ParsedMatchWardInfos.Where(x => x.AccountId == accountId).Select(x => x.MatchId).ToListAsync();

	/// <inheritdoc />
	public async Task<bool> IsMatchParsedAsync(long matchId, long accountId) =>
		await _dbContext.ParsedMatchWardInfos.AnyAsync(x => x.MatchId == matchId && x.AccountId == accountId);

	/// <inheritdoc />
	public async Task RemoveAsync(long matchId, long accountId)
	{
		_dbContext.ParsedMatchWardInfos.Remove(new ParsedMatchWardInfo
		{
			MatchId = matchId,
			AccountId = accountId
		});
		await _dbContext.SaveChangesAsync();
	}
}

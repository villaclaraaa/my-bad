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
			PlayedAtDateUtc = playedAtDateUtc.ToUniversalTime()
		});
		await _dbContext.SaveChangesAsync();
	}

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

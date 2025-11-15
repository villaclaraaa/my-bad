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
	public async Task AddAsync(long matchId, DateTime playedAtDateUtc)
	{
		await _dbContext.ParsedMatchWardInfos.AddAsync(new ParsedMatchWardInfo
		{
			MatchId = matchId,
			PlayedAtDateUtc = playedAtDateUtc
		});
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<bool> IsMatchParsedAsync(long matchId) =>
		await _dbContext.ParsedMatchWardInfos.AnyAsync(x => x.MatchId == matchId);

	/// <inheritdoc />
	public async Task RemoveAsync(long matchId)
	{
		_dbContext.ParsedMatchWardInfos.Remove(new ParsedMatchWardInfo
		{
			MatchId = matchId
		});
		await _dbContext.SaveChangesAsync();
	}
}

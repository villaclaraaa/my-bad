using Microsoft.EntityFrameworkCore;
using Mybad.Core.DomainModels;
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
	public async Task AddAsync(ParsedMatchWardInfoModel model)
	{
		await _dbContext.ParsedMatchWardInfos.AddAsync(new ParsedMatchWardInfo
		{
			MatchId = model.MatchId,
			AccountId = model.AccountId,
			IsRadiantPlayer = model.IsRadiantPlayer,
			IsWonMatch = model.IsWonMatch,
			PlayedAtDateUtc = model.PlayedAtDateUtc
		});
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task AddRangeAsync(IEnumerable<ParsedMatchWardInfoModel> list)
	{
		await _dbContext.AddRangeAsync(list.Select(x => new ParsedMatchWardInfo
		{
			MatchId = x.MatchId,
			AccountId = x.AccountId,
			IsRadiantPlayer = x.IsRadiantPlayer,
			IsWonMatch = x.IsWonMatch,
			PlayedAtDateUtc = x.PlayedAtDateUtc
		}));
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<IEnumerable<long>> GetParsedMatchesForAccountAsync(long accountId, bool? isForRadiant)
	{
		var query = _dbContext.ParsedMatchWardInfos.AsQueryable();

		query = query.Where(x => x.AccountId == accountId);

		if (isForRadiant != null)
		{
			query = query.Where(x => x.IsRadiantPlayer == isForRadiant);
		}

		return await query.Select(x => x.MatchId).ToListAsync();
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

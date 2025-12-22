using Microsoft.EntityFrameworkCore;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services;

public class CheckedMatchesService : ICheckedMatchesService
{
	private readonly ApplicationDbContext _context;

	public CheckedMatchesService(ApplicationDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc />
	public long CheckedMatchesCount => _context.CheckedMatches.LongCount();

	/// <inheritdoc />
	public async Task AddCheckedMatches(List<long> checkedMatches)
	{
		var entries = checkedMatches.Select(id => new CheckedMatchMatchupEntity { MatchId = id });
		_context.CheckedMatches.AddRange(entries);
		await _context.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<List<long>> FilterAlreadyCheckedMatches(List<long> matches)
	{
		var existingIds = await _context.CheckedMatches
		 .AsNoTracking()
		.Where(m => matches.Contains(m.MatchId))
		.Select(m => m.MatchId)
		.ToListAsync();

		var newMatchIds = matches.Except(existingIds).ToList();

		return newMatchIds;
	}
}

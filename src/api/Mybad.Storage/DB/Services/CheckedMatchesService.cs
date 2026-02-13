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
    public async Task AddCheckedMatches(List<long> checkedMatches, int patchId)
    {
        var entries = checkedMatches.Select(id => new CheckedMatchMatchupEntity { MatchId = id, PatchId = patchId });
        _context.CheckedMatches.AddRange(entries);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<long>> FilterAlreadyCheckedMatches(List<long> matches)
    {
        var recentCheckedMatches = (await _context.CheckedMatches
        .AsNoTracking()
        .OrderByDescending(m => m.MatchId)
        .Take(100)
        .Select(m => m.MatchId)
        .ToListAsync()).ToHashSet();

        return matches.Where(m => !recentCheckedMatches.Contains(m)).ToList();
    }
}

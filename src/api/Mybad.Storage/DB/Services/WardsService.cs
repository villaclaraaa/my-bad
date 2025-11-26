using Microsoft.EntityFrameworkCore;
using Mybad.Core.DomainModels;
using Mybad.Core.Services;
using Mybad.Storage.DB.Mappers;

namespace Mybad.Storage.DB.Services;

/// <summary>
/// Defines ward-related operations with storage using EFCore.
/// </summary>
public class WardsService : IWardService
{
	private readonly ApplicationDbContext _dbContext;

	public WardsService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc/>
	public async Task AddAsync(WardModel ward)
	{
		int deviation = 2;  // this is approximate value of units where we can consider wards placed in same position

		// check if ward in similar position already added
		var existingWard = _dbContext.Wards.FirstOrDefault(x =>
			x.MatchId == ward.MatchId &&
			x.AccountId == ward.AccountId &&
			Math.Abs(x.PosX - ward.PosX) <= deviation &&
			Math.Abs(x.PosY - ward.PosY) <= deviation);

		// if found, just increase amount
		if (existingWard != null)
		{
			existingWard.Amount++;
			existingWard.TimeLivedSeconds = existingWard.TimeLivedSeconds + ward.TimeLivedSeconds;
			existingWard.WasDestroyed = existingWard.WasDestroyed || ward.WasDestroyed;
			await _dbContext.SaveChangesAsync();
			return;
		}

		// otherwise, add new ward
		var entity = ward.MapToEntity();

		_dbContext.Wards.Add(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task AddRangeAsync(IEnumerable<WardModel> wards)
	{
		var entities = wards.Select(x => x.MapToEntity());
		_dbContext.Wards.AddRange(entities);
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<WardModel>> GetAllByMatchAsync(long matchId, long accountId) =>
		await _dbContext.Wards.Where(x => x.MatchId == matchId && x.AccountId == accountId)
			.Select(x => x.MapToModel()).ToListAsync();

	/// <inheritdoc/>
	public async Task<IEnumerable<WardModel>> GetAllForAccountAsync(long accountId) =>
		await _dbContext.Wards.Where(x => x.AccountId == accountId)
			.Select(x => x.MapToModel()).ToListAsync();

	/// <inheritdoc/>
	public async Task DeleteAllForAccountAsync(long accountId)
	{
		var wards = _dbContext.Wards.Where(x => x.AccountId == accountId);
		_dbContext.Wards.RemoveRange(wards);
		await _dbContext.SaveChangesAsync();
	}

	/// <inheritdoc/>
	public async Task DeleteAllFromMatchAsync(long matchId)
	{
		var wards = _dbContext.Wards.Where(x => x.MatchId == matchId);
		_dbContext.Wards.RemoveRange(wards);
		await _dbContext.SaveChangesAsync();
	}
}

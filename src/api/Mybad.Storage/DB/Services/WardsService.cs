using Microsoft.EntityFrameworkCore;
using Mybad.Core.DomainModels;
using Mybad.Core.Services;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services;

public class WardsService : IWardService
{
	private readonly ApplicationDbContext _dbContext;

	public WardsService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task AddAsync(WardModel ward)
	{
		int deviation = 2;

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
			await _dbContext.SaveChangesAsync();
			return;
		}

		// otherwise, add new ward
		var entity = new WardEntity
		{
			PosX = ward.PosX,
			PosY = ward.PosY,
			Amount = ward.Amount,
			MatchId = ward.MatchId,
			AccountId = ward.AccountId,
			TimeLivedSeconds = ward.TimeLivedSeconds,
			WasDestroyed = ward.WasDestroyed,
			CreatedDate = DateTime.UtcNow
		};

		_dbContext.Wards.Add(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteAllForAccountAsync(long accountId)
	{
		try
		{
			var wards = _dbContext.Wards.Where(x => x.AccountId == accountId);
			_dbContext.Wards.RemoveRange(wards);
			await _dbContext.SaveChangesAsync();
		}
		catch
		{
			// ignore
		}
	}

	public async Task DeleteAllFromMatchAsync(long matchId)
	{
		var wards = _dbContext.Wards.Where(x => x.MatchId == matchId);
		_dbContext.Wards.RemoveRange(wards);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<WardModel>> GetAllByMatchAsync(long matchId) =>
		await _dbContext.Wards.Where(x => x.MatchId == matchId)
			.Select(x => new WardModel
			{
				PosX = x.PosX,
				PosY = x.PosY,
				Amount = x.Amount,
				MatchId = x.MatchId,
				AccountId = x.AccountId,
				TimeLivedSeconds = x.TimeLivedSeconds,
				WasDestroyed = x.WasDestroyed
			}).ToListAsync();

	public async Task<IEnumerable<WardModel>> GetAllForAccountAsync(long accountId) =>
		await _dbContext.Wards.Where(x => x.AccountId == accountId)
			.Select(x => new WardModel
			{
				PosX = x.PosX,
				PosY = x.PosY,
				Amount = x.Amount,
				MatchId = x.MatchId,
				AccountId = x.AccountId,
				TimeLivedSeconds = x.TimeLivedSeconds,
				WasDestroyed = x.WasDestroyed
			}).ToListAsync();
}

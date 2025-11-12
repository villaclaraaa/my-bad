using Mybad.Core.Models;
using Mybad.Core.Services;

namespace Mybad.Storage.DB.Services;

public class WardsService : IWardService
{
	private readonly ApplicationDbContext _dbContext;

	public WardsService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async void AddAsync(WardModel ward)
	{
		var entity = new WardEntity
		{
			Id = ward.Id,
			Name = ward.Name,
			HospitalId = ward.HospitalId
		};

		_dbContext.Wards.Add(entity);
		await _dbContext.SaveChangesAsync();
	}
}

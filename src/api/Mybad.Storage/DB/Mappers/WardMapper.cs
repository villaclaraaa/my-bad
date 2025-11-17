using Mybad.Core.DomainModels;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Mappers;

internal static class WardMapper
{
	public static WardEntity MapToEntity(this WardModel ward) =>
		new()
		{
			CreatedDate = DateTime.UtcNow,
			PosX = ward.PosX,
			PosY = ward.PosY,
			AccountId = ward.AccountId,
			MatchId = ward.MatchId,
			Amount = ward.Amount,
			TimeLivedSeconds = ward.TimeLivedSeconds,
			WasDestroyed = ward.WasDestroyed
		};

	public static WardModel MapToModel(this WardEntity ward) =>
		new()
		{
			PosX = ward.PosX,
			PosY = ward.PosY,
			AccountId = ward.AccountId,
			MatchId = ward.MatchId,
			Amount = ward.Amount,
			TimeLivedSeconds = ward.TimeLivedSeconds,
			WasDestroyed = ward.WasDestroyed
		};
}

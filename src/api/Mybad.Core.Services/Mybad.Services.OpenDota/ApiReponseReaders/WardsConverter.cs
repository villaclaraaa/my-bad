using Mybad.Core.DomainModels;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;

namespace Mybad.Services.OpenDota.ApiResponseReaders;

internal class WardsConverter
{
	/// <summary>
	/// Converts the incoming <see cref="WardPlacementMap"/> Api response to a tuple of List of Observers and Sentries
	/// wards of <see cref="Ward"/> type.
	/// </summary>
	/// <param name="apiReponse">ODota api response model.</param>
	/// <returns>Tuple of lists of <see cref="Ward"/> type objects.</returns>
	public (List<Ward> obses, List<Ward> sens) ConvertToWardList(WardPlacementMap apiReponse)
	{
		var obses = new List<Ward>();
		var sens = new List<Ward>();
		foreach (var (x, innerDic) in apiReponse.Obs)
		{
			foreach (var (y, amount) in innerDic)
			{
				obses.Add(new Ward
				{
					X = int.Parse(x),
					Y = int.Parse(y),
					Amount = amount
				});
			}
		}

		foreach (var (x, innerDic) in apiReponse.Sen)
		{
			foreach (var (y, amount) in innerDic)
			{
				sens.Add(new Ward
				{
					X = int.Parse(x),
					Y = int.Parse(y),
					Amount = amount,
				});
			}
		}

		return (obses, sens);
	}


	/// <summary>
	/// Converts the <see cref="MatchWardLogInfo"/> API response to a <see cref="WardLog"/> models.
	/// </summary>
	/// <param name="apiReponse">ODota api response model.</param>
	/// <param name="accountId">Account for user to check wards.</param>
	/// <returns>Tuple of lists of <see cref="WardLog"/> objects.</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public (List<WardLog> obses, List<WardLog> sens) ConvertWardsLogMatch(MatchWardLogInfo apiReponse, long accountId)
	{
		var playerInfo = apiReponse.Players.FirstOrDefault(x => x.AccountId == accountId);
		if (playerInfo == null)
		{
			throw new InvalidOperationException();
		}

		var obses = ConvertWards(playerInfo.ObsLog, playerInfo.ObsLeftLog, isObs: true);
		var sens = ConvertWards(playerInfo.SenLog, playerInfo.SenLeftLog, isObs: false);

		return (obses, sens);
	}

	/// <summary>
	/// Internal function to convert only one type of wards into list of <see cref="WardLog"/>.
	/// </summary>
	/// <param name="wardLog">List of wards placed.</param>
	/// <param name="wardLeftLog">List of wards destroyed.</param>
	/// <param name="isObs">True if the previous params is about Observer Wards, if false - then its about Sentries.</param>
	/// <returns>List of <see cref="WardLog"/> objects.</returns>
	private List<WardLog> ConvertWards(List<WardLogEntry> wardLog, List<WardLeftLogEntry> wardLeftLog, bool isObs = true)
	{
		var response = new List<WardLog>();
		var defaultTime = isObs ? 360 : 420;

		foreach (var ward in wardLog)
		{
			var timeLived = defaultTime;    // Default duration - 6mins.
			if (wardLeftLog.FirstOrDefault(x => x.EHandle == ward.EHandle) is WardLogEntry leftLogEntry)
			{
				/* 
				 * Calculating time depending on whether ward was placed and destroyed before/after horn.
				 */
				// put = -50, destroy = -20 => lived = 30s
				if (leftLogEntry.Time < 0 && ward.Time < 0)
				{
					timeLived = Math.Abs(ward.Time) - Math.Abs(leftLogEntry.Time);
				}

				// put = -20, destroy = 20 => lived = 40s
				if (leftLogEntry.Time < 0 && ward.Time > 0)
				{
					timeLived = leftLogEntry.Time + Math.Abs(ward.Time);
				}

				// put = 20, destroy = 40 => lived = 20s
				timeLived = leftLogEntry.Time - ward.Time;
			}
			var wardL = new WardLog
			{
				X = (int)Math.Round(ward.X, MidpointRounding.AwayFromZero),
				Y = (int)Math.Round(ward.Y, MidpointRounding.AwayFromZero),
				TimeLived = timeLived > defaultTime ? defaultTime : timeLived,
				WasDestroyed = timeLived != defaultTime,
				Amount = 1 // TODO - amount of ward when creating WardsLogMatchResponse.
						   // Dont know if i should increase count for all wards in same place.
			};
			response.Add(wardL);
		}

		return response;
	}

	/// <summary>
	/// Converts <see cref="MatchWardLogInfo"/> object containing wards object as obs_log and obs_left_log
	/// to the lit of <see cref="WardModel"/> objects. 
	/// </summary>
	/// <param name="apiReponse">Api response object.</param>
	/// <param name="accountId">Account Id for which user to convert.</param>
	/// <param name="matchId">MatchId. Used to create <see cref="WardModel"/> object.</param>
	/// <param name="isObs">Sets whether this method should convert Observers or Sentries. Defaults to <c>true</c>.</param>
	/// <returns><see cref="List{T}"/> of <see cref="WardModel"/> objects.</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public List<WardModel> ConvertWardsToWardModel(MatchWardLogInfo apiReponse, long accountId, long matchId, bool isObs = true)
	{
		var playerInfo = apiReponse.Players.FirstOrDefault(x => x.AccountId == accountId);
		if (playerInfo == null)
		{
			throw new InvalidOperationException();
		}

		var wardLog = playerInfo.ObsLog;
		var wardLeftLog = playerInfo.ObsLeftLog;

		var response = new List<WardModel>();
		var defaultTime = isObs ? 360 : 420;

		foreach (var ward in wardLog)
		{
			var timeLived = defaultTime;    // Default duration - 6mins.
			if (wardLeftLog.FirstOrDefault(x => x.EHandle == ward.EHandle) is WardLogEntry leftLogEntry)
			{
				/* 
				 * Calculating time depending on whether ward was placed and destroyed before/after horn.
				 */
				// put = -50, destroy = -20 => lived = 30s
				if (leftLogEntry.Time < 0 && ward.Time < 0)
				{
					timeLived = Math.Abs(ward.Time) - Math.Abs(leftLogEntry.Time);
				}

				// put = -20, destroy = 20 => lived = 40s
				if (leftLogEntry.Time < 0 && ward.Time > 0)
				{
					timeLived = leftLogEntry.Time + Math.Abs(ward.Time);
				}

				// put = 20, destroy = 40 => lived = 20s
				timeLived = leftLogEntry.Time - ward.Time;
			}
			var wardL = new WardModel
			{
				PosX = (int)Math.Round(ward.X, MidpointRounding.AwayFromZero),
				PosY = (int)Math.Round(ward.Y, MidpointRounding.AwayFromZero),
				TimeLivedSeconds = timeLived > defaultTime ? defaultTime : timeLived,
				WasDestroyed = timeLived != defaultTime,
				Amount = 1,
				MatchId = matchId,
				AccountId = accountId
			};
			response.Add(wardL);
		}

		return response;
	}
}

using Mybad.Core.DomainModels;
using Mybad.Core.Responses.Entries;
using Mybad.Services.OpenDota.ApiResponseModels;

namespace Mybad.Services.OpenDota.ApiResponseReaders;

internal class WardsPlacementMapReader
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
	/// Converts the list of <see cref="MatchWardLogInfo"/> API response to a <see cref="WardLog"/> models.
	/// </summary>
	/// <param name="mathesLogs">List of ODota api responses.</param>
	/// <param name="accountId">Account for uset to check wards.</param>
	/// <returns>Tuple of lists of <see cref="WardLog"/> objects.</returns>
	/// <remarks>It is basically the same as <see cref="ConvertWardsLogMatch(MatchWardLogInfo, long)"/> but for list.</remarks>
	/// <exception cref="InvalidOperationException"></exception>
	public (List<WardLog> obses, List<WardLog> sens) ConvertWardsLogManyMathes(List<MatchWardLogInfo> mathesLogs, long accountId)
	{
		var obses = new List<WardLog>();
		var sens = new List<WardLog>();

		Dictionary<(int X, int Y), WardLog> dicObs = [];
		Dictionary<(int X, int Y), WardLog> dicSen = [];

		foreach (var log in mathesLogs)
		{
			var playerInfo = log.Players.FirstOrDefault(x => x.AccountId == accountId);
			if (playerInfo == null)
			{
				throw new InvalidOperationException();
			}

			obses.AddRange(ConvertWards(playerInfo.ObsLog, playerInfo.ObsLeftLog, true));
			sens.AddRange(ConvertWards(playerInfo.SenLog, playerInfo.SenLeftLog, false));
		}

		foreach (var obs in obses)
		{
			(int x, int y) key = (obs.X, obs.Y);
			if (!dicObs.TryGetValue(key, out WardLog? value))
			{
				dicObs.Add(key, obs);
			}
			else
			{
				value.Amount++;
				value.TimeLived = (value.TimeLived + obs.TimeLived) / value.Amount;
			}
		}

		foreach (var sen in sens)
		{
			(int x, int y) key = (sen.X, sen.Y);
			if (!dicSen.TryGetValue(key, out WardLog? value))
			{
				dicSen.Add(key, sen);
			}
			else
			{
				value.Amount++;
			}
		}

		return ([.. dicObs.Select(x => x.Value)], [.. dicSen.Select(x => x.Value)]);
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

	public List<WardModel> ConvertWardsToWardModel(List<WardLogEntry> wardLog, List<WardLeftLogEntry> wardLeftLog, bool isObs = true)
	{
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
				TimeLived = timeLived > defaultTime ? defaultTime : timeLived,
				WasDestroyed = timeLived != defaultTime,
				Amount = 1 // TODO - amount of ward when creating WardsLogMatchResponse.
						   // Dont know if i should increase count for all wards in same place.
			};
			response.Add(wardL);
		}

		return response;
	}
}

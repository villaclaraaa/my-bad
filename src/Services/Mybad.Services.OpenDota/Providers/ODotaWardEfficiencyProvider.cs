using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;

namespace Mybad.Services.OpenDota.Providers;

public class ODotaWardEfficiencyProvider : IInfoProvider<WardsEfficiencyRequest, WardsEfficiencyResponse>
{
	public Task<WardsEfficiencyResponse> GetInfo(WardsEfficiencyRequest request)
	{
		throw new NotImplementedException();
	}
}

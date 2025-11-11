namespace Mybad.Core;

/// <summary>
/// Contract for receiving data.
/// </summary>
/// <typeparam name="TRequest">Request model to send.</typeparam>
/// <typeparam name="TResponse">Response model retrieved from provider.</typeparam>
/// <remarks>
/// All new providers should implement this interface.
/// </remarks>
public interface IInfoProvider<TRequest, TResponse>
	where TRequest : BaseRequest
	where TResponse : BaseResponse
{
	/// <summary>
	/// Gets the needed information.
	/// </summary>
	/// <param name="request">Request model instance, inherited from <see cref="BaseRequest"/>.</param>
	/// <returns>Response model object, inherited from <see cref="BaseResponse"/>.</returns>
	Task<TResponse> GetInfo(TRequest request);
}

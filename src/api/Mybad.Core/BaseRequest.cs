namespace Mybad.Core;

/// <summary>
/// Base model for any request you want to send. Is passed as parameter inside <see cref="IInfoProvider{TRequest, TResponse}"/>.
/// It contains common parameters for every request.
/// <para>
/// <b>Any new reqeust model should inherit this.</b>
/// </para>
/// </summary>
public abstract class BaseRequest
{
	/// <summary>
	/// Gets or sets id of the request.
	/// </summary>
	public int RequestId { get; set; }

	/// <summary>
	/// Gets or sets account id of the player for which you want to perform the request.
	/// </summary>
	public virtual long? AccountId { get; set; }
}
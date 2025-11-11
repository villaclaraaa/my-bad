namespace Mybad.Core;

/// <summary>
/// Base model for any response you want to retrieve.
/// Contains common properties for each and every response.
/// <para>
/// <b>Any new response model should inherit this.</b>
/// </para>
/// </summary>
public abstract class BaseResponse
{
	public int Id { get; set; }
}

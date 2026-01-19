using System.Security.Cryptography;
using System.Text;

namespace Mybad.API.Endpoints;

/// <summary>
/// An endpoint filter that enforces API key authentication for incoming HTTP requests.
/// </summary>
/// <remarks>This should be added into endpoints to which we want to restrict unathorized access as AddEndpointFilter<"ApiKeyEndpointFilter">().</remarks>
public class ApiKeyEndpointFilter : IEndpointFilter
{
	private const string HeaderName = "X-Api-Access";
	private const string QueryName = "apiKey";

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var http = context.HttpContext;
		var config = http.RequestServices.GetRequiredService<IConfiguration>();

		var expected = config["ApiKeys:XApiAccess"];
		if (string.IsNullOrEmpty(expected))
		{
			return Results.Problem("API key not configured", statusCode: 500);
		}

		var provided = http.Request.Headers[HeaderName].FirstOrDefault()
						?? http.Request.Query[QueryName].FirstOrDefault();

		if (string.IsNullOrEmpty(provided) || !SecureEquals(provided, expected))
		{
			return Results.Unauthorized();
		}

		return await next(context);
	}

	private static bool SecureEquals(string a, string b)
	{
		var aBytes = Encoding.UTF8.GetBytes(a);
		var bBytes = Encoding.UTF8.GetBytes(b);
		return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
	}
}

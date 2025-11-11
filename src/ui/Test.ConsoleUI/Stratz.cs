using System.Net;
using System.Net.Http.Headers;
using System.Text;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Test.ConsoleUI;

public class Stratz
{
	public async Task DefaultClient()
	{
		var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJTdWJqZWN0IjoiNDVmMzc4YWEtZjU5Yy00NzEwLWJmMDUtMjJlYjQzZjA5ZDFhIiwiU3RlYW1JZCI6IjEzNjk5NjA4OCIsIkFQSVVzZXIiOiJ0cnVlIiwibmJmIjoxNzYxMjA2ODY1LCJleHAiOjE3OTI3NDI4NjUsImlhdCI6MTc2MTIwNjg2NSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3RyYXR6LmNvbSJ9.Ia4oNCtRDeziR5n_190ZLxHuQg_N53bFCZFB03vlSD8";


		var handler = new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
		};

		using var client = new HttpClient(handler)
		{
			DefaultRequestVersion = HttpVersion.Version20,
			DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher

		};
		client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", token);
		client.DefaultRequestHeaders.UserAgent.Clear();
		client.DefaultRequestHeaders.UserAgent.Add(
			new ProductInfoHeaderValue("STRATZ_API", "1.0")
		);
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json")
		);
		client.DefaultRequestHeaders.ExpectContinue = false;

		var json = "{\"query\":\"query { constants { heroes { id displayName } } }\"}";
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await client.PostAsync("https://api.stratz.com/graphql", content);
		var result = await response.Content.ReadAsStringAsync();
		Console.WriteLine(response.StatusCode);
		Console.WriteLine(result);
	}

	public async Task GraphQl()
	{
		var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJTdWJqZWN0IjoiNDVmMzc4YWEtZjU5Yy00NzEwLWJmMDUtMjJlYjQzZjA5ZDFhIiwiU3RlYW1JZCI6IjEzNjk5NjA4OCIsIkFQSVVzZXIiOiJ0cnVlIiwibmJmIjoxNzYxMjA2ODY1LCJleHAiOjE3OTI3NDI4NjUsImlhdCI6MTc2MTIwNjg2NSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3RyYXR6LmNvbSJ9.Ia4oNCtRDeziR5n_190ZLxHuQg_N53bFCZFB03vlSD8";

		var handler = new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
		};

		using var graphClient = new GraphQLHttpClient(
			new GraphQLHttpClientOptions { EndPoint = new Uri("https://api.stratz.com/graphql") },
			new NewtonsoftJsonSerializer(),
			new HttpClient(handler)
		);
		graphClient.HttpClient.DefaultRequestHeaders.UserAgent.Clear();
		graphClient.HttpClient.DefaultRequestHeaders.UserAgent.Add(
			new ProductInfoHeaderValue(new ProductHeaderValue("STRATZ_API"))
		);

		graphClient.HttpClient.DefaultRequestHeaders.Accept.Clear();
		graphClient.HttpClient.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json")
		);
		// Correct headers
		//graphClient.HttpClient.DefaultRequestHeaders.Authorization =
		//	new AuthenticationHeaderValue("Bearer", token);
		graphClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		graphClient.HttpClient.DefaultRequestHeaders.ExpectContinue = false;

		var request = new GraphQLRequest
		{
			Query = """
                query GetForPlayer($id: Long!) {
                    player(steamAccountId: $id) {
                        matches(request: {take: 5}) {
                            id
                        }
                    }
                }
                """,
			Variables = new
			{
				id = 136996088
			},
			OperationName = "GetForPlayer"
		};

		try
		{
			var response = await graphClient.SendQueryAsync<dynamic>(request);
			Console.WriteLine(response.Data);
		}
		catch (GraphQLHttpRequestException ex)
		{
			Console.WriteLine(ex.StatusCode + $"{ex.Content}");
			foreach (var a in ex.ResponseHeaders)
			{
				Console.WriteLine(a.Key + " " + a.Value);
			}
		}
	}
}

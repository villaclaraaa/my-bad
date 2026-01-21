using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Mybad.API.Endpoints;
using Mybad.API.Services;
using Mybad.Core;
using Mybad.Core.Providers.CoreHeroMatchupProvider;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Services;
using Mybad.Services.OpenDota;
using Mybad.Services.OpenDota.Cachers;
using Mybad.Storage.DB;
using Mybad.Storage.DB.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

/* 
 * Db registration section. 
 */
var con = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(con));

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

// Setup API only DbContext (such as TgBot etc maybe)
builder.Services.AddDbContext<ApiDbContext>(options =>
	options.UseNpgsql(con));

//builder.Services.AddDbContext<ApiDbContext>(options =>
//    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

/* 
 * Projects services registration.
 */
/* Mybad.Core services registration. */
builder.Services.AddScoped<IInfoProvider<HeroMatchupRequest, HeroMatchupResponse>, CoreHeroMatchupProvider>();

/* Mybad.Storage.Db services registration. */
builder.Services.AddDbServices();

/* ODota Services registration including httpclient and info providers. */
builder.Services.AddODotaServices();

/* 
 * API (current project) services registration. 
 */
builder.Services.AddSingleton<HeroMatchupCacherStatus>();

// Register some stuff only in Prod env.
if (!builder.Environment.IsDevelopment())
{
	builder.Services.AddHostedService<HeroMatchupCacherHostedService>();

	// Configuring Rate Limiting Middleware
	var timespanSeconds = builder.Configuration.GetValue<int>("RateLimit:TimespanSeconds");
	var requestCount = builder.Configuration.GetValue<int>("RateLimit:RequestsCount");
	builder.Services.AddRateLimiter(options =>
	{
		options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			RateLimitPartition.GetFixedWindowLimiter(
				partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
				factory: partition => new FixedWindowRateLimiterOptions
				{
					PermitLimit = requestCount,
					Window = TimeSpan.FromSeconds(timespanSeconds)
				}));

		options.OnRejected = async (context, token) =>
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			context.HttpContext.Response.Headers.RetryAfter = $"{timespanSeconds}";
			await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", token);
		};
	});
}

// Setup TgBot News
var bot_token = builder.Configuration["BotSettings:Tg_BotToken"]!;
var webhookURL = builder.Configuration["BotSettings:Tg_BotWebhookUrl"]!;
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(bot_token!, httpClient));
builder.Services.AddScoped<INotifier, TgBotNotifier>();
builder.Services.AddScoped<TgBotSubscriberService>();

// OpenApi + Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Added by Andrew due to a problem sending a request to Api)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularApp", policy =>
	{
		policy.WithOrigins("http://localhost:63512") // Angular app URL
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});
/*
 * End of services registartion.
 */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Angular in the same project setup
app.UseDefaultFiles();
app.UseStaticFiles();

//Here as well
app.UseCors("AllowAngularApp");

if (!app.Environment.IsDevelopment())
{
	app.UseRateLimiter();
}

app.MapWardEndpoints();
app.MapMatchupEndpoints();
app.MapPlayerEndpoints();

app.MapTgBotEndpoints(webhookURL);

app.MapGet("/test", () => "alive alive ya, xarosh");
app.MapGet("/cacheOnce", async (ODotaHeroMatchupCacher cacher) =>
{
	await cacher.UpdateHeroMatchupsDatabase(75);
	return "success";
})
	.AddEndpointFilter<ApiKeyEndpointFilter>();

app.MapFallbackToFile("index.html");

app.Run();

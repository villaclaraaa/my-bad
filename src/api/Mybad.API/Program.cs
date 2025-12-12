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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Db registration
var con = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(con));

// Core services + Db implementations
builder.Services.AddScoped<IInfoProvider<HeroMatchupRequest, HeroMatchupResponse>, CoreHeroMatchupProvider>();

builder.Services.AddScoped<IWardService, WardsService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();
builder.Services.AddScoped<ICheckedMatchesService, CheckedMatchesService>();
builder.Services.AddScoped<IParsedMatchWardInfoService, ParsedMatchWardInfoService>();

// ODota Services registration including httpclient and info providers.
builder.Services.AddODotaServices();

builder.Services.AddSingleton<HeroMatchupCacherStatus>();
builder.Services.AddHostedService<HeroMatchupCacherHostedService>();

// Setup API only DbContext (such as TgBot etc maybe)
builder.Services.AddDbContext<ApiDbContext>(options =>
	options.UseNpgsql(con));

// Setup TgBot News
var bot_token = builder.Configuration["BotSettings:Tg_BotToken"]!;
var webhookURL = builder.Configuration["BotSettings:Tg_BotWebhookUrl"]!;
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(bot_token!, httpClient));
builder.Services.AddScoped<INotifier, TgBotNotifier>();
builder.Services.AddScoped<TgBotSubscriberService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapWardEndpoints();
app.MapMatchupEndpoints();

app.MapGet("/test", () => "xarosh")
	.AllowAnonymous();

app.MapGet("/cache", async (ODotaHeroMatchupCacher cacher) =>
{
	await cacher.UpdateHeroMatchupsDatabase(75);
	return "success";
});

app.MapTgBotEndpoints(webhookURL);

app.Run();

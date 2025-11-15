using Microsoft.EntityFrameworkCore;
using Mybad.API.Endpoints;
using Mybad.Core.Services;
using Mybad.Services.OpenDota;
using Mybad.Storage.DB;
using Mybad.Storage.DB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Db registration
var con = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(con));

// Core services + Db registration
builder.Services.AddScoped<IWardService, WardsService>();
builder.Services.AddScoped<IParsedMatchWardInfoService, ParsedMatchWardInfoService>();

// ODota Services registration including httpclient and info providers.
builder.Services.AddODotaServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapWardEndpoints();

app.Run();

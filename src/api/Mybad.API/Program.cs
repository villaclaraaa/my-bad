using Microsoft.EntityFrameworkCore;
using Mybad.API.Endpoints;
using Mybad.Core;
using Mybad.Core.Requests;
using Mybad.Core.Responses;
using Mybad.Core.Services;
using Mybad.Services.OpenDota.Providers;
using Mybad.Storage.DB;
using Mybad.Storage.DB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IInfo Providers registration
builder.Services.AddScoped<IInfoProvider<WardMapRequest, WardsMapPlacementResponse>, ODotaWardPlacementMapProvider>();

// Db registration
var con = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(con));

// Core services + Db registration
builder.Services.AddScoped<IWardService, WardsService>();

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

using Carter;
using URLShortener.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCarter()
    .AddApplicationDbContext(configuration)
    .AddUrlShortenerService()
    .AddRedis(configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.ApplyDatabaseMigrations();

app.MapCarter();

app.Run();
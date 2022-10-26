using BouncingRectangles.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHostedService<CoordinatesGeneratorService>();

var app = builder.Build();

app.MapGrpcService<BouncingRectangeService>();

app.MapGet("/", () => "Hello World!");

app.Run();

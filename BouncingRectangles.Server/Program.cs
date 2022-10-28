using BouncingRectangles.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<ICoordinatesGeneratorService, CoordinatesGeneratorService>();
builder.Services.AddSingleton<IRectangleFactory, RectangleFactory>();

var app = builder.Build();

app.MapGrpcService<BouncingRectangeGrpcService>();

app.MapGet("/", () => "Hello World!");

app.Run();

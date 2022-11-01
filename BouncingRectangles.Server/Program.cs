using BouncingRectangles.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<CoordinatesGeneratorService>();
builder.Services.AddSingleton<IRectanglesFactory, RectanglesFactory>();
builder.Services.AddSingleton<ITaskCountDeterminator, TaskCountDeterminator>();
builder.Services.AddTransient<IRectangleSubscriber, RectangleSubscriber>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<BouncingRectangeGrpcService>();
app.MapGet("/", () => "Hello World!");

app.Run();

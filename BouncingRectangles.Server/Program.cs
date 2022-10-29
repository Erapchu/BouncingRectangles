using BouncingRectangles.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CoordinatesGeneratorService>();
builder.Services.AddHostedService<CoordinatesGeneratorService>(s => s.GetRequiredService<CoordinatesGeneratorService>());
builder.Services.AddSingleton<ICoordinatesGeneratorService>(s => s.GetRequiredService<CoordinatesGeneratorService>());
builder.Services.AddSingleton<IRectangleFactory, RectangleFactory>();
builder.Services.AddSingleton<IRectangleSubscriberFactory, RectangleSubscriberFactory>();
builder.Services.AddTransient<IRectangleSubscriber, RectangleSubscriber>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<BouncingRectangeGrpcService>();
app.MapGet("/", () => "Hello World!");

app.Run();

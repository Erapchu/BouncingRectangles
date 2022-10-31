using BouncingRectangles.Server.Models;
using BouncingRectangles.Server.Protos;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public class BouncingRectangeGrpcService : Protos.BouncingRectangesDistributor.BouncingRectangesDistributorBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BouncingRectangeGrpcService> _logger;

        public BouncingRectangeGrpcService(
            IServiceProvider serviceProvider,
            ILogger<BouncingRectangeGrpcService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override async Task Subscribe(
            SubscribeRequestDto request,
            IServerStreamWriter<BouncingRectangleUpdateDto> responseStream,
            ServerCallContext context)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var subscriber = scope.ServiceProvider.GetService<IRectangleSubscriber>();
            if (subscriber is null)
                return;
            subscriber.Update += async (sender, e) =>
                await WriteUpdateAsync(responseStream, e.Rectangles);

            _logger.LogInformation("Subscription started");
            // Just awaits for client closes the connection/interrupted somehow
            await WaitForCancellation(context.CancellationToken);
            _logger.LogInformation("Subscription finished");
        }

        private async Task WriteUpdateAsync(IServerStreamWriter<BouncingRectangleUpdateDto> stream, IEnumerable<Rectangle> rectangles)
        {
            try
            {
                var bouncingRectangleUpdate = new BouncingRectangleUpdateDto();
                foreach (var rect in rectangles)
                {
                    // Fill bouncing rectangles dto
                    bouncingRectangleUpdate.Rectangles.Add(new BouncingRectangleDto()
                    {
                        Height = rect.Height,
                        Width = rect.Width,
                        X = rect.X,
                        Y = rect.Y,
                        Id = rect.Id.ToString(),
                    });
                }
                await stream.WriteAsync(bouncingRectangleUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to write message: {ex.Message}");
            }
        }

        private static Task WaitForCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }
    }
}

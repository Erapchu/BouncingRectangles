using BouncingRectangles.Protos;
using BouncingRectangles.Server.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public class BouncingRectangeGrpcService : Protos.BouncingRectangeHost.BouncingRectangeHostBase
    {
        private readonly IRectangleSubscriberFactory _rectangleSubscriberFactory;
        private readonly ILogger<BouncingRectangeGrpcService> _logger;

        public BouncingRectangeGrpcService(
            IRectangleSubscriberFactory rectangleSubscriberFactory,
            ILogger<BouncingRectangeGrpcService> logger)
        {
            _rectangleSubscriberFactory = rectangleSubscriberFactory;
            _logger = logger;
        }

        public override async Task Subscribe(
            SubscribeRequestDto request,
            IServerStreamWriter<BouncingRectangleUpdateDto> responseStream,
            ServerCallContext context)
        {
            var subscriber = _rectangleSubscriberFactory.GetSubscriber();
            subscriber.Update += async (sender, e) =>
                await WriteUpdateAsync(responseStream, e.Rectangles);

            _logger.LogInformation("Subscription started");
            // Just awaits for client closes the connection/interrupted somehow
            await WaitForCancellation(context.CancellationToken);
            subscriber.Dispose(); // Dispose subscriber
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

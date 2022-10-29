using BouncingRectangles.Server.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public interface IRectangleSubscriber : IDisposable
    {
        event EventHandler<RectangleUpdateEventArgs> Update;
    }

    public class RectangleSubscriber : IRectangleSubscriber
    {
        public event EventHandler<RectangleUpdateEventArgs> Update;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _task;
        private readonly TimeSpan _updateTimeout = TimeSpan.FromMilliseconds(500);
        private readonly ILogger<RectangleSubscriber> _logger;
        private readonly ICoordinatesGeneratorService _coordinatesGeneratorService;

        public RectangleSubscriber(
            ILogger<RectangleSubscriber> logger,
            ICoordinatesGeneratorService coordinatesGeneratorService)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _task = RunNotify(_cancellationTokenSource.Token);
            _logger = logger;
            _coordinatesGeneratorService = coordinatesGeneratorService;
        }

        private async Task RunNotify(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (Update != null)
                    {
                        var rectangles = _coordinatesGeneratorService.GetRectangles();
                        Update.Invoke(this, new RectangleUpdateEventArgs(rectangles));
                    }
                    await Task.Delay(_updateTimeout, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore - it's cancelled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}

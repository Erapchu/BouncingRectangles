using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public class CoordinatesGeneratorService : IHostedService
    {
        private readonly ILogger<CoordinatesGeneratorService> _logger;
        private readonly TimeSpan _generateInterval = TimeSpan.FromSeconds(1);
        private CancellationTokenSource _cancellationTokenSource;

        public CoordinatesGeneratorService(ILogger<CoordinatesGeneratorService> logger)
        {
            _logger = logger;
        }

        private async Task GenereateCoordinates(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Generate coordinates

                    await Task.Delay(_generateInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var rnd = new Random();
            var tasksCount = rnd.Next(Environment.ProcessorCount);
            var token = _cancellationTokenSource.Token;
            for (int i = 0; i < tasksCount; i++)
            {
                Task.Run(() => GenereateCoordinates(token), token);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            return Task.CompletedTask;
        }
    }
}

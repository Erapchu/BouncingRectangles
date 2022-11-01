using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public class CoordinatesGeneratorService : IHostedService
    {
        private readonly ILogger<CoordinatesGeneratorService> _logger;
        private readonly IRectanglesFactory _rectangleFactory;
        private readonly ITaskCountDeterminator _taskCountDeterminator;
        private readonly TimeSpan _generateInterval = TimeSpan.FromMilliseconds(100);
        private readonly List<Task> _tasks = new();
        private CancellationTokenSource _cancellationTokenSource;

        private int MaxX { get; } = Constants.FieldWidth - Constants.RectangleWidth;
        private int MaxY { get; } = Constants.FieldHeight - Constants.RectangleHeight;

        public CoordinatesGeneratorService(
            ILogger<CoordinatesGeneratorService> logger,
            IRectanglesFactory rectangleFactory,
            ITaskCountDeterminator taskCountDeterminator)
        {
            _logger = logger;
            _rectangleFactory = rectangleFactory;
            _taskCountDeterminator = taskCountDeterminator;
        }

        private async Task GenereateCoordinates(CancellationToken cancellationToken)
        {
            try
            {
                var id = Guid.NewGuid();
                if (_rectangleFactory.CreateRectanglesGroup(id, out var rectanglesGroup))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        foreach (var rectangle in rectanglesGroup.GetItems())
                        {
                            rectangle.X = RandomNumberGenerator.GetInt32(MaxX);
                            rectangle.Y = RandomNumberGenerator.GetInt32(MaxY);
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await Task.Delay(_generateInterval, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogWarning("Can't create rectangles group with id '{0}'", id);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var tasksCount = _taskCountDeterminator.GetCount();
            var token = _cancellationTokenSource.Token;
            for (int i = 0; i < tasksCount; i++)
            {
                _tasks.Add(Task.Run(() => GenereateCoordinates(token), token));
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

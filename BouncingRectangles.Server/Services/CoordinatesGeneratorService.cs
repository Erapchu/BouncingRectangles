using BouncingRectangles.Server.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public interface ICoordinatesGeneratorService
    {
        public IEnumerable<Rectangle> GetRectangles();
    }

    public class CoordinatesGeneratorService : ICoordinatesGeneratorService, IHostedService
    {
        private readonly ILogger<CoordinatesGeneratorService> _logger;
        private readonly IRectangleFactory _rectangleFactory;
        private readonly TimeSpan _generateInterval = TimeSpan.FromMilliseconds(200);
        private readonly Dictionary<Guid, Rectangle> _coordinatedRects = new();
        private readonly List<Task> _tasks = new();
        private CancellationTokenSource _cancellationTokenSource;

        private int MaxX { get; } = Constants.FieldWidth - Constants.RectangleWidth;
        private int MaxY { get; } = Constants.FieldHeight - Constants.RectangleHeight;

        public CoordinatesGeneratorService(
            ILogger<CoordinatesGeneratorService> logger,
            IRectangleFactory rectangleFactory)
        {
            _logger = logger;
            _rectangleFactory = rectangleFactory;
        }

        private async Task GenereateCoordinates(CancellationToken cancellationToken)
        {
            try
            {
                var id = Guid.NewGuid();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var rectanglesGroup = _rectangleFactory.GetRectanglesGroup(id);
                    if (rectanglesGroup is not null)
                    {
                        foreach (var rectangle in rectanglesGroup.Rectangles)
                        {
                            rectangle.X = RandomNumberGenerator.GetInt32(MaxX);
                            rectangle.Y = RandomNumberGenerator.GetInt32(MaxY);
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        lock (_coordinatedRects)
                        {
                            _coordinatedRects.Remove(rectanglesGroup.Id);
                            _coordinatedRects.Add(rectanglesGroup.Id, rectanglesGroup);
                        }
                    }

                    await Task.Delay(_generateInterval, cancellationToken);
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
            var rnd = new Random();
            var tasksCount = rnd.Next(Environment.ProcessorCount);
            _rectangleFactory.SetGroupsCount(tasksCount);

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

        public IEnumerable<Rectangle> GetRectangles()
        {
            List<Rectangle> rectanglesList = null;
            lock (_coordinatedRects)
            {
                rectanglesList = _coordinatedRects.Values.ToList();
            }
            return rectanglesList;
        }
    }
}

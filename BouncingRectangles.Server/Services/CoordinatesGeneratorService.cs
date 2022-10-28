using BouncingRectangles.Common;
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
        private readonly TimeSpan _generateInterval = TimeSpan.FromSeconds(1);
        private readonly Dictionary<Guid, Rectangle> _coordinatedRects = new();
        private CancellationTokenSource _cancellationTokenSource;

        public CoordinatesGeneratorService(
            ILogger<CoordinatesGeneratorService> logger,
            IRectangleFactory rectangleFactory)
        {
            _logger = logger;
            _rectangleFactory = rectangleFactory;
        }

        private async Task GenereateCoordinates(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var rectangle = _rectangleFactory.GetRectangle();
                    rectangle.X = RandomNumberGenerator.GetInt32(Constants.FieldWidth);
                    rectangle.Y = RandomNumberGenerator.GetInt32(Constants.FieldHeight);
                    
                    lock (_coordinatedRects)
                    {
                        _coordinatedRects.Remove(rectangle.Id);
                        _coordinatedRects.Add(rectangle.Id, rectangle);
                    }

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

using BouncingRectangles.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BouncingRectangles.Server.Services
{
    public interface IRectanglesFactory
    {
        public IEnumerable<Rectangle> GetRectangles();
        public bool CreateRectanglesGroup(Guid id, out Group<Rectangle> rectanglesGroup);
    }

    public class RectanglesFactory : IRectanglesFactory
    {
        private readonly Dictionary<Guid, Group<Rectangle>> _rectanglesGroups = new();
        private readonly int _rectanglesCount;
        private readonly object _lock = new();
        private readonly int _maxGroupsCount;
        private readonly int _batchSize;

        public RectanglesFactory(ITaskCountDeterminator taskCountDeterminator)
        {
            var rnd = new Random();
            _rectanglesCount = rnd.Next(100, 1000);
            _maxGroupsCount = taskCountDeterminator.GetCount();
            _batchSize = _rectanglesCount / _maxGroupsCount;
        }

        public bool CreateRectanglesGroup(Guid id, out Group<Rectangle> rectanglesGroup)
        {
            lock (_lock)
            {
                var result = false;
                if (_rectanglesGroups.Count < _maxGroupsCount)
                {
                    int capacity = _rectanglesGroups.Count == _maxGroupsCount - 1
                        ? _rectanglesCount - _batchSize * (_maxGroupsCount - 1) // last group case
                        : _batchSize; // usual group
                    rectanglesGroup = new Group<Rectangle>(id, capacity);
                    result = _rectanglesGroups.TryAdd(id, rectanglesGroup);
                }
                else
                {
                    // Can't add more
                    rectanglesGroup = null;
                }

                return result;
            }
        }

        public IEnumerable<Rectangle> GetRectangles()
        {
            var rectangles = new List<Rectangle>(_rectanglesCount);
            IEnumerable<Rectangle> extracted;
            lock (_lock)
            {
                extracted = _rectanglesGroups.Values.SelectMany(r => r.Items);
            }
            rectangles.AddRange(extracted);
            return rectangles;
        }
    }
}

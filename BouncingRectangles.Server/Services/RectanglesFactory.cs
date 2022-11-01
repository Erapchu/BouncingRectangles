using BouncingRectangles.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BouncingRectangles.Server.Services
{
    public interface IRectanglesFactory
    {
        public IEnumerable<Rectangle> GetRectangles();
        public bool CreateRectanglesGroup(Guid id, out Group rectanglesGroup);
    }

    public class RectanglesFactory : IRectanglesFactory
    {
        private readonly HashSet<Group> _rectanglesGroups = new();
        private readonly int _rectanglesCount;
        private readonly object _lock = new();
        private readonly int _maxGroupsCount;
        private readonly int _batchSize;
        private int _overloadedCount;

        public RectanglesFactory(ITaskCountDeterminator taskCountDeterminator)
        {
            var rnd = new Random();
            _rectanglesCount = rnd.Next(100, 1000);
            _maxGroupsCount = taskCountDeterminator.GetCount();
            _batchSize = _rectanglesCount / _maxGroupsCount;
            _overloadedCount = _rectanglesCount - _batchSize * _maxGroupsCount;
        }

        public bool CreateRectanglesGroup(Guid id, out Group rectanglesGroup)
        {
            var result = false;
            lock (_lock)
            {
                if (_rectanglesGroups.Count < _maxGroupsCount)
                {
                    int capacity = _batchSize;
                    if (_overloadedCount > 0)
                    {
                        if (_rectanglesGroups.Count == _maxGroupsCount - 1) // Last group
                        {
                            capacity += _overloadedCount;
                            _overloadedCount = 0;
                        }
                        else // Usual group
                        {
                            capacity++;
                            _overloadedCount--;
                        }
                    }
                    rectanglesGroup = new Group(id, capacity);
                    result = _rectanglesGroups.Add(rectanglesGroup);
                }
                else
                {
                    // Can't add more
                    rectanglesGroup = null;
                }
            }

            if (result)
                rectanglesGroup.RefreshItems();

            return result;
        }

        public IEnumerable<Rectangle> GetRectangles()
        {
            var rectangles = new List<Rectangle>(_rectanglesCount);
            IEnumerable<Rectangle> extracted;
            lock (_lock)
            {
                extracted = _rectanglesGroups.SelectMany(g => g.GetItems()).ToList();
            }
            rectangles.AddRange(extracted);
            return rectangles;
        }
    }
}

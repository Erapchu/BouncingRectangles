using BouncingRectangles.Server.Models;
using System;
using System.Collections.Generic;

namespace BouncingRectangles.Server.Services
{
    public interface IRectangleFactory
    {
        public RectanglesGroup GetRectanglesGroup(Guid id);
        public void SetGroupsCount(int count);
    }

    public class RectangleFactory : IRectangleFactory
    {
        private readonly HashSet<RectanglesGroup> _rectanglesGroups = new();
        private readonly int _rectanglesCount;
        private int _groupsCount;

        private int BatchCount
        {
            get
            {

            }
        }

        public RectangleFactory()
        {
            var rnd = new Random();
            _rectanglesCount = rnd.Next(100, 1000);
        }

        public RectanglesGroup GetRectanglesGroup(Guid id)
        {
            if (_groupsCount == 0)
                return null;

            
        }

        public void SetGroupsCount(int count)
        {
            _groupsCount = count;
        }
    }
}

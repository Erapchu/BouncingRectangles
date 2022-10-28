using BouncingRectangles.Server.Models;
using System;
using System.Collections.Generic;

namespace BouncingRectangles.Server.Services
{
    public interface IRectangleFactory
    {
        public Rectangle GetRectangle();
    }

    public class RectangleFactory : IRectangleFactory
    {
        private readonly List<Rectangle> _rectangles = new();
        private int _pointer = 0;

        public RectangleFactory()
        {
            var rnd = new Random();
            var rectanglesCount = rnd.Next(100, 1000);
            for (int i = 0; i < rectanglesCount; i++)
            {
                _rectangles.Add(Rectangle.CreateNew());
            }
        }

        public Rectangle GetRectangle()
        {
            lock (_rectangles)
            {
                if (_pointer > _rectangles.Count)
                    _pointer = 0;

                var rectangle = _rectangles[_pointer];
                _pointer++;
                return rectangle;
            }
        }
    }
}

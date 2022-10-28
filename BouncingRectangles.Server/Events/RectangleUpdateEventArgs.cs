using BouncingRectangles.Server.Models;
using System;
using System.Collections.Generic;

namespace BouncingRectangles.Server.Events
{
    public class RectangleUpdateEventArgs : EventArgs
    {
        public IEnumerable<Rectangle> Rectangles { get; }

        public RectangleUpdateEventArgs(IEnumerable<Rectangle> rectangle)
        {
            Rectangles = rectangle;
        }
    }
}

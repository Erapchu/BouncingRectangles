using System;
using System.Collections.Generic;

namespace BouncingRectangles.Server.Models
{
    public class RectanglesGroup
    {
        public Guid Id { get; }
        public List<Rectangle> Rectangles { get; }

        public RectanglesGroup(Guid id, int count)
        {
            Id = id;
            Rectangles = new List<Rectangle>(count);
        }
    }
}

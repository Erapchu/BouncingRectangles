using System;
using System.Diagnostics;
using System.Threading;

namespace BouncingRectangles.Server.Models
{
    [DebuggerDisplay("w:{Width}, h:{Height}, x:{X}, y:{Y}")]
    public class Rectangle
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public Guid Id { get; init; }

        private int _x;
        public int X
        {
            get => Interlocked.CompareExchange(ref _x, 0, 0);
            set => Interlocked.Exchange(ref _x, value);
        }

        private int _y;
        public int Y
        {
            get => Interlocked.CompareExchange(ref _y, 0, 0);
            set => Interlocked.Exchange(ref _y, value);
        }

        private Rectangle()
        {
        }

        public static Rectangle CreateNew()
        {
            return new Rectangle()
            {
                Width = Constants.RectangleWidth,
                Height = Constants.RectangleHeight,
                Id = Guid.NewGuid(),
            };
        }
    }
}

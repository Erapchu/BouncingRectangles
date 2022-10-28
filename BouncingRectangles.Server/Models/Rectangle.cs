﻿using System;
using System.Threading;

namespace BouncingRectangles.Server.Models
{
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
                Width = 50,
                Height = 50,
                Id = Guid.NewGuid(),
            };
        }
    }
}
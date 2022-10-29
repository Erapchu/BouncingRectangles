using BouncingRectangles.Server.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BouncingRectangles.WPF.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class RectangleViewModel
    {
        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        public Guid Id { get; }

        public RectangleViewModel()
        {

        }

        public RectangleViewModel(BouncingRectangleDto bouncingRectangleDto)
        {
            if (bouncingRectangleDto is null)
                throw new ArgumentNullException(nameof(bouncingRectangleDto));

            _x = bouncingRectangleDto.X;
            _y = bouncingRectangleDto.Y;
            _width = bouncingRectangleDto.Width;
            _height = bouncingRectangleDto.Height;
            if (Guid.TryParse(bouncingRectangleDto.Id, out Guid guid))
            {
                Id = guid;
            }
        }

        public void Update(BouncingRectangleDto bouncingRectangleDto)
        {
            X = bouncingRectangleDto.X;
            Y = bouncingRectangleDto.Y;
        }
    }
}

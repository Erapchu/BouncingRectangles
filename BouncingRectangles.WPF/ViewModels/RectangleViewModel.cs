using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BouncingRectangles.WPF.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class RectangleViewModel
    {
        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private int _width;

        [ObservableProperty]
        private int _height;
        
        public Guid Id { get; }

        public RectangleViewModel()
        {

        }
    }
}

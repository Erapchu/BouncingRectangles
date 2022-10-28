using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace BouncingRectangles.WPF.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class MainViewModel
    {
        private static readonly Lazy<MainViewModel> _lazyDesignTimeInstance = new(GetDesignTimeInstance);
        public static MainViewModel DesignTimeInstance => _lazyDesignTimeInstance.Value;

        private static MainViewModel GetDesignTimeInstance()
        {
            var mvm = new MainViewModel();
            mvm.RectangleVMs.Add(new RectangleViewModel()
            {
                Width = 50,
                Height = 50,
                X = 0,
                Y = 0,
            });
            mvm.RectangleVMs.Add(new RectangleViewModel()
            {
                Width = 50,
                Height = 50,
                X = 50,
                Y = 50,
            });
            return mvm;
        }

        public ObservableCollection<RectangleViewModel> RectangleVMs { get; set; } = new();

        public MainViewModel()
        {
            
        }
    }
}

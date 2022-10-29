using BouncingRectangles.Server.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BouncingRectangles.WPF.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class MainViewModel
    {
        #region Design
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
        #endregion

        private readonly Dictionary<Guid, RectangleViewModel> _rectanglesVMMap = new();

        [ObservableProperty]
        private bool _loading;

        public ObservableCollection<RectangleViewModel> RectangleVMs { get; set; } = new();

        public MainViewModel()
        {

        }

        [RelayCommand]
        private async Task Loaded()
        {
            try
            {
                Loading = true;

                await Task.Run(() =>
                {
                    using var channel = GrpcChannel.ForAddress("https://localhost:7115");
                    var client = new BouncingRectangesDistributor.BouncingRectangesDistributorClient(channel);

                    var request = new SubscribeRequestDto();
                    request.Id = Guid.NewGuid().ToString();
                    using var stream = client.Subscribe(request);

                    var cts = new CancellationTokenSource();
                    var task = DisplayAsync(stream.ResponseStream, cts.Token);
                });
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task DisplayAsync(IAsyncStreamReader<BouncingRectangleUpdateDto> stream, CancellationToken token)
        {
            try
            {
                await foreach (var updateDto in stream.ReadAllAsync(/*token*/))
                {
                    if (updateDto is null)
                        continue;

                    foreach (var rectangle in updateDto.Rectangles)
                    {
                        if (!Guid.TryParse(rectangle.Id, out Guid rectId))
                            continue;

                        if (_rectanglesVMMap.TryGetValue(rectId, out var rectangleVM))
                        {
                            // Existing
                            rectangleVM?.Update(rectangle);
                        }
                        else
                        {
                            // Add new
                            _rectanglesVMMap.Add(rectId, new RectangleViewModel(rectangle));
                        }
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Display finished.");
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
    }
}

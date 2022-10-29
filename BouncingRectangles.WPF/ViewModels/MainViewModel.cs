using BouncingRectangles.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Grpc.Net.Client;
using System;
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
                    using var channel = GrpcChannel.ForAddress("http://localhost:5115");
                    var client = new BouncingRectangles.Protos.BouncingRectangesDistributor.BouncingRectangesDistributorClient(channel);

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
                await foreach (var updateDto in stream.ReadAllAsync(token))
                {
                    if (updateDto is null)
                        continue;


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

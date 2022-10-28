using Microsoft.Extensions.DependencyInjection;
using System;

namespace BouncingRectangles.Server.Services
{
    public interface IRectangleSubscriberFactory
    {
        IRectangleSubscriber GetSubscriber();
    }

    public class RectangleSubscriberFactory : IRectangleSubscriberFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RectangleSubscriberFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRectangleSubscriber GetSubscriber()
        {
            return _serviceProvider.GetService<RectangleSubscriber>();
        }
    }
}

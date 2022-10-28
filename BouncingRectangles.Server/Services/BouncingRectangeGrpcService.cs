using BouncingRectangles.Protos;
using Grpc.Core;
using System.Threading.Tasks;

namespace BouncingRectangles.Server.Services
{
    public class BouncingRectangeGrpcService : Protos.BouncingRectangeHost.BouncingRectangeHostBase
    {
        public override Task Subscribe(
            SubscribeRequest request,
            IServerStreamWriter<BouncingRectangleUpdate> responseStream,
            ServerCallContext context)
        {
            return base.Subscribe(request, responseStream, context);
        }
    }
}

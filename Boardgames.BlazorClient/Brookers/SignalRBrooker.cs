using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;

namespace Boardgames.BlazorClient.Brookers
{
    public class SignalRBrooker : ISignalRBrooker
    {
        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
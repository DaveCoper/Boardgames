using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.Client.Brookers
{
    public interface ISignalRBrooker
    { 
        Task ConnectAsync(CancellationToken cancellationToken);
    }
}
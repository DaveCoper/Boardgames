using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.Wpf.Client.Services
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
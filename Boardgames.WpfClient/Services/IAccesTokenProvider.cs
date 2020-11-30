using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.WpfClient.Services
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
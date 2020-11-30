using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.Client.Brookers
{
    public interface IWebApiBrooker
    {
        Task<TReturnType> GetAsync<TReturnType>(
            string controllerName, 
            string actionName = null, 
            IEnumerable<KeyValuePair<string, string>> parameters = null, 
            CancellationToken cancellationToken = default);

        Task<TReturnType> PostAsync<TReturnType, TContentType>(
            string controllerName,
            TContentType content,
            string actionName = null,
            CancellationToken cancellationToken = default);
    }
}
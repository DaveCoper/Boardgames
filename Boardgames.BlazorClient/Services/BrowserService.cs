using Boardgames.BlazorClient.Models;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Boardgames.BlazorClient.Services
{
    public class BrowserService
    {
        private readonly IJSRuntime _js;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<BrowserDimension> GetDimensionsAsync()
        {
            return await _js.InvokeAsync<BrowserDimension>("getDimensions");
        }
    }
}
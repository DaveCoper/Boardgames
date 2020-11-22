using System.Threading.Tasks;

namespace Boardgames.Wpf.Client.Services
{
    internal interface ISecureStore
    {
        Task<string> ReadStringAsync(string filePath);
        Task StoreStringAsync(string data, string filePath);
    }
}
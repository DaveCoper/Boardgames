using System;
using System.Threading.Tasks;

namespace Boardgames.Shared.Services
{
    public interface IGameMessenger
    {
        Task ConnectToGameAsync(int gameId);

        void RegisterMessageHandler(Action<object> handler, bool keepAlive = false);
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Boardgames.WebServer.Hubs
{
    public class GameHub : Hub
    {
        public async Task SubscribeToGameMessagesAsync(int gameId)
        {
            await Task.Yield();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
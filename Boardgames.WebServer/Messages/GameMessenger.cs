using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.WebServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.WebServer.Messages
{
    public class GameMessenger : IGameMessenger
    {
        private readonly GameType gameType;

        private readonly IHubContext<GameHub> hubContext;

        private readonly ILogger<GameMessenger> logger;

        private List<SignalRGameMessage> messageBuffer = new List<SignalRGameMessage>();

        public GameMessenger(
            GameType gameType,
            IHubContext<GameHub> hubContext,
            ILogger<GameMessenger> logger)
        {
            this.gameType = gameType;
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            this.logger = logger ?? NullLogger<GameMessenger>.Instance;
        }

        public void SendMessage<TMessageType>(TMessageType message, params int[] receiverPlayerIds) where TMessageType : IGameMessage
        {
            this.SendMessage(message, (IEnumerable<int>)receiverPlayerIds);
        }

        public void SendMessage<TMessageType>(TMessageType message, IEnumerable<int> receiverPlayerIds) where TMessageType : IGameMessage
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            foreach (var receiver in receiverPlayerIds)
            {
                messageBuffer.Add(new SignalRGameMessage { ReceiverId = receiver, Payload = message });
            }
        }

        public async Task FlushAsync()
        {
            await Task.WhenAll(messageBuffer.Select(x => SendMessageAsync(x)));
        }

        private async Task<bool> SendMessageAsync(SignalRGameMessage msg)
        {
            var receiverId = msg.ReceiverId.ToString(CultureInfo.InvariantCulture);
            var connection = hubContext.Clients.User(receiverId);

            var payloadType = msg.Payload.GetType().Name;

            if (connection != null)
            {
                try
                {
                    await connection.SendAsync($"{gameType}_{payloadType}", msg.Payload);
                    return true;
                }
                catch (Exception e)
                {
                    logger.LogError($"Failed to send message over SignalR. GameId:{msg.Payload.GameId}, Message type: {payloadType}, Receiver {receiverId}", e);
                }
            }
            else
            {
                logger.LogError($"Failed to send message over SignalR. No SignalR connection to user. GameId:{msg.Payload.GameId}, Message type: {payloadType}, Receiver {receiverId}");
            }

            return false;
        }

    }
}
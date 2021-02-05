using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.Client.Messages
{
    /// <summary>
    /// This piece of shit has to be replaced with something better.
    /// </summary>
    public class MessageRouter
    {
        private readonly Dictionary<int, IMessageRouter> gameRouters;

        private readonly ILogger<MessageRouter> logger;

        public MessageRouter(ILogger<MessageRouter> logger)
        {
            this.gameRouters = new Dictionary<int, IMessageRouter>();
            this.logger = logger ?? NullLogger<MessageRouter>.Instance;
        }

        public async Task RouteMessage(IGameMessage gameMessage)
        {
        
                if (gameRouters.TryGetValue(gameMessage.GameId, out var gameRouter))
                {
                    await gameRouter.RouteMessage(gameMessage);
                }

        }

        public void RegisterGameRouter(IMessageRouter messageRouter)
        {
                gameRouters.TryAdd(messageRouter.GameId, messageRouter);
             
        }
    }
}
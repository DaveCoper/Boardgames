using System;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Client.Messages;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace Boardgames.BlazorClient.Brookers
{
    public class SignalRBrooker : ISignalRBrooker
    {
        private readonly Uri baseAddress;

        private readonly MessageRouter messageQueue;

        private readonly IAccessTokenProvider accessTokenProvider;

        private HubConnection hub;

        public SignalRBrooker(
            Uri baseAddress,
            MessageRouter messageQueue,
            IAccessTokenProvider accessTokenProvider)
        {
            this.baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            this.messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
            this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (this.hub != null)
                return;

            var url = new Uri(baseAddress, $"/hubs/GameHub");
            var hub = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var result = await accessTokenProvider.RequestAccessToken();

                        if (result.TryGetToken(out var token))
                        {
                            return token.Value;
                        }

                        throw new Exception();
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            await hub.StartAsync(cancellationToken);

            this.RegiserMessage<NewPlayerConnected>(
                hub,
                GameType.NinthPlanet);

            this.RegiserMessage<PlayerHasLeft>(
                hub,
                GameType.NinthPlanet);

            this.RegiserMessage<GameHasStarted>(
                hub,
                GameType.NinthPlanet);

            this.RegiserMessage<PlayerCommunicatedCard>(
                hub,
                GameType.NinthPlanet);

            this.RegiserMessage<CardWasPlayed>(
                hub,
                GameType.NinthPlanet);

            this.hub = hub;
        }

        private void RegiserMessage<T>(HubConnection hub, GameType gameType)
            where T : IGameMessage
        {
            hub.On<T>(
                FromatMethodName(gameType, typeof(T)),
                async msg =>
                {
                    Console.WriteLine($"Message of type {msg.GetType().Name} arrived.");
                    await this.messageQueue.RouteMessage(msg);
                });
        }

        private string FromatMethodName(GameType gameType, Type messageType)
        {
            string gamePrefix = null;

            switch (gameType)
            {
                case GameType.NinthPlanet:
                    gamePrefix = "NinthPlanet";
                    break;
            }

            return $"{gamePrefix}_{messageType.Name}";
        }
    }
}
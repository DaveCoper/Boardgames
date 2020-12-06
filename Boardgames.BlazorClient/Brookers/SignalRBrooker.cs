using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace Boardgames.BlazorClient.Brookers
{
    public class SignalRBrooker : ISignalRBrooker
    {
        private readonly Uri baseAddress;

        private readonly IMessenger messenger;

        private readonly IAccessTokenProvider accessTokenProvider;

        private HubConnection hub;

        public SignalRBrooker(Uri baseAddress, IMessenger messenger, IAccessTokenProvider accessTokenProvider)
        {

            this.baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
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

            this.hub = hub;
        }

        private void RegiserMessage<T>(HubConnection hub, GameType gameType)
        {
            hub.On<T>(
                FromatMethodName(gameType, typeof(T)),
                msg =>
                {
                    Console.WriteLine($"Message of type {msg.GetType().Name} arrived.");
                    this.messenger.Send(msg);
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
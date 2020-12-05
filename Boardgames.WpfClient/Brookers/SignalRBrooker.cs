﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Boardgames.Client.Brookers;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Boardgames.WpfClient.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Boardgames.WpfClient.Brookers
{
    public class SignalRBrooker : ISignalRBrooker
    {
        private readonly Dispatcher dispatcher;

        private readonly IAccessTokenProvider accessTokenProvider;

        private readonly IMessenger messenger;

        private HubConnection hub;

        public SignalRBrooker(Dispatcher dispatcher, IAccessTokenProvider accessTokenProvider, IMessenger messenger)
        {
            messenger.Register<SubscribeToGameMessages>(this, OnUserWantsToSubscribe);

            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            var baseUrl = new Uri("https://localhost:44399/");

            var url = new Uri(baseUrl, $"/hubs/GameHub?access_token={accessToken}");
            var hub = new HubConnectionBuilder()
                .WithUrl(url)
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
                    dispatcher.InvokeAsync(() =>
                    {
                        this.messenger.Send(msg);
                    });
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

        private void OnUserWantsToSubscribe(SubscribeToGameMessages obj)
        {
            var result = this.hub.SendAsync("SubscribeToGameMessages", obj.GameId);
            HandleAsyncResult(result);
        }

        private void HandleAsyncResult(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    break;

                case TaskStatus.Faulted:
                    Debug.WriteLine($"Failed to send message. Error: {task.Exception.Message}");
                    break;

                case TaskStatus.Canceled:
                    Debug.WriteLine($"Sending of message was canceled.");
                    break;
            }
        }
    }
}
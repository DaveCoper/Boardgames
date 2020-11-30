using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Boardgames.WpfClient.Brookers
{
    public class SignalRBrooker
    {
        private readonly Dispatcher dispatcher;

        private readonly IMessenger messenger;

        private HubConnection hub;

        public SignalRBrooker(Dispatcher dispatcher, IMessenger messenger)
        {
            messenger.Register<SubscribeToGameMessages>(this, OnUserWantsToSubscribe);

            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            var hub = new HubConnectionBuilder()
                .WithUrl("")
                .WithAutomaticReconnect()
                .Build();

            await hub.StartAsync(cancellationToken);
            hub.On<NewPlayerConnected>("NinthPlanetPlayerConnected", OnPlayerConnected);
            hub.On<PlayerHasLeft>("NinthPlanetPlayerLeft", OnPlayerLeft);
            hub.On<GameHasStarted>("NinthPlanetGameStarted", OnGameStarted);
            this.hub = hub;
        }

        private void OnGameStarted(GameHasStarted message)
        {
            dispatcher.InvokeAsync(() =>
            {
                this.messenger.Send(message);
            });
        }

        private void OnPlayerLeft(PlayerHasLeft message)
        {
            dispatcher.InvokeAsync(() =>
            {
                this.messenger.Send(message);
            });
        }

        private void OnPlayerConnected(NewPlayerConnected message)
        {
            dispatcher.InvokeAsync(() =>
            {
                this.messenger.Send(message);
            });
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
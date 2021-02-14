using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Boardgames.BlazorClient.Services;
using Boardgames.Client.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Boardgames.BlazorClient.Components.NinthPlanet
{
    public partial class NpLocalPlayer : ComponentBase, IDisposable
    {
        private UserState localPlayer;

        [Inject]
        public DragDropDataStore DragDropStore { get; set; }

        [Inject]
        public INinthPlanetService NinthPlanetService { get; set; }

        [Parameter]
        public UserState LocalPlayer
        {
            get => localPlayer;
            set
            {
                if (localPlayer != value)
                {
                    if (localPlayer != null)
                    {
                        localPlayer.PropertyChanged -= OnModelPropertyChanged;
                    }

                    localPlayer = value;

                    if (value != null)
                    {
                        localPlayer.PropertyChanged += OnModelPropertyChanged;
                    }
                }
            }
        }

        [Parameter]
        public EventCallback<Card> CardDroppedToCommunicationArea { get; set; }

        [Parameter]
        public EventCallback<TaskCard> CardDroppedToDeckArea { get; set; }

        public async Task OnDropToCommunication(DragEventArgs args)
        {
            if (DragDropStore.DragDropData is Card card)
            {
                await CardDroppedToCommunicationArea.InvokeAsync(card);
            }
        }

        public async Task OnDropToDeck(DragEventArgs args)
        {
            if (this.LocalPlayer.IsOnTurn && DragDropStore.DragDropData is TaskCard card)
            {
                await CardDroppedToDeckArea.InvokeAsync(card);
            }
        }

        void IDisposable.Dispose()
        {
            this.LocalPlayer = null;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.StateHasChanged();
        }
    }
}
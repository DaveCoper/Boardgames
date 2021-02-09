using System.Threading.Tasks;
using Boardgames.BlazorClient.Extensions;
using Boardgames.BlazorClient.Services;
using Boardgames.Client.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Boardgames.BlazorClient.Components.NinthPlanet
{
    public partial class NpLocalPlayer : ComponentBase
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
                localPlayer = value;
                if (localPlayer != null)
                {
                    localPlayer.UpdateOnPropertyChanged(this.StateHasChanged);
                    localPlayer.CardsInHand.UpdateOnCollectionChanged<Card>(this.StateHasChanged);
                    localPlayer.FinishedTasks.UpdateOnCollectionChanged<TaskCard>(this.StateHasChanged);
                    localPlayer.UnfinishedTasks.UpdateOnCollectionChanged<TaskCard>(this.StateHasChanged);
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
            if (DragDropStore.DragDropData is TaskCard card)
            {
                await CardDroppedToDeckArea.InvokeAsync(card);
            }
        }
    }
}
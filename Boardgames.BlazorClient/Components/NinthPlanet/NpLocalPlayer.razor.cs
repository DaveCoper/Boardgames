using Boardgames.BlazorClient.Extensions;
using Boardgames.BlazorClient.Services;
using Boardgames.Client.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

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
    }
}
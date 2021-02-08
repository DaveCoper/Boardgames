using Boardgames.BlazorClient.Extensions;
using Boardgames.BlazorClient.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Models;
using Boardgames.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Boardgames.BlazorClient.Components.NinthPlanet
{
    public partial class NpGameRound : ComponentBase
    {
        private GameRound gameRound;

        [Inject]
        public Services.DragDropDataStore dragDropStore { get; set; }

        [Inject]
        public INinthPlanetService ninthPlanetService { get; set; }

        [Parameter]
        public GameRound GameRound
        {
            get => gameRound;
            set
            {
                gameRound = value;
                if (gameRound != null)
                {
                    gameRound.UpdateOnPropertyChanged(this.StateHasChanged);
                    gameRound.AvailableGoals.UpdateOnCollectionChanged<TaskCard>(this.StateHasChanged);
                }
            }
        }

        [Parameter]
        public int GameId { get; set; }

        public async Task OnDropToPlayArea(DragEventArgs args)
        {
            if (dragDropStore.DragDropData is Card card)
            {
                await ninthPlanetService.PlayCardAsync(this.GameId, card);
            }
        }
    }
}
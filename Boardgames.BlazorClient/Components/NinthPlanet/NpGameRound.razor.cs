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
    public partial class NpGameRound : ComponentBase
    {
        private GameRound gameRound;

        [Inject]
        public DragDropDataStore dragDropStore { get; set; }

        [Inject]
        public INinthPlanetService NinthPlanetService { get; set; }

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
                await NinthPlanetService.PlayCardAsync(this.GameId, card);
            }
        }

        private async Task CardDroppedToCommunicationArea(Card card)
        {
            await NinthPlanetService.DisplayCardAsync(this.GameId, card, null);
        }

        private async Task CardDroppedToDeckArea(TaskCard taskCard)
        {
            await NinthPlanetService.TakeGoalAsync(this.GameId, taskCard);
        }
    }
}
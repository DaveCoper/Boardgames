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
                if (gameRound != value)
                {
                    if (gameRound != null)
                    {
                        value.PropertyChanged -= OnModelPropertyChanged;
                    }

                    gameRound = value;

                    if (value != null)
                    {
                        value.PropertyChanged += OnModelPropertyChanged;
                    }
                }
            }
        }

        [Parameter]
        public int GameId { get; set; }

        public string PlayAreaBackground => this.GameRound.UserCanPlay ? "#d8d8d8ff" : "#ffffff00";

        public async Task OnDropToPlayArea(DragEventArgs args)
        {
            if (dragDropStore.DragDropData is Card card)
            {
                await NinthPlanetService.PlayCardAsync(this.GameId, card);
            }
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.StateHasChanged();
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
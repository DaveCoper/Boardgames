using System.Linq;
using System.Threading.Tasks;
using Boardgames.BlazorClient.Extensions;
using Boardgames.Client.Brookers;
using Boardgames.Client.Factories;
using Boardgames.Client.Messages;
using Boardgames.Client.ViewModels;
using Boardgames.Client.ViewModels.NinthPlanet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Boardgames.BlazorClient.Pages
{
    [Authorize]
    public partial class NinthPlanet : ComponentBase
    {
        [Inject]
        public MainViewModel mainVm { get; set; }

        [Inject]
        public INinthPlanetScreenViewModelFactory vmFactory { get; set; }

        [Inject]
        public MessageRouter messageQueue { get; set; }

        [Inject]
        public ISignalRBrooker signalR { get; set; }

        [Parameter]
        public int GameId { get; set; }

        public NinthPlanetScreenViewModel ScreenVm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await signalR.ConnectAsync();

                var vm = mainVm.Screens
                    .OfType<NinthPlanetScreenViewModel>()
                    .FirstOrDefault(x => x.GameId == this.GameId);

                if (vm == null)
                {
                    vm = vmFactory.CreateInstance(GameId);
                    mainVm.Screens.Add(vm);
                }

                mainVm.ActiveScreen = vm;

                await vm.LoadDataAsync();
                var router = new Boardgames.NinthPlanet.Client.ClientMessageRouter(vm.Client);
                messageQueue.RegisterGameRouter(router);


                vm.UpdateOnPropertyChanged(this.StateHasChanged);
                vm.Client.UpdateOnPropertyChanged(this.StateHasChanged);

                this.ScreenVm = vm;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
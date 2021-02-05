using Boardgames.Common.Models;
using Boardgames.Common.Observables;
using GalaSoft.MvvmLight;

namespace Boardgames.NinthPlanet.Client
{
    public class GameLobby : ObservableObject
    {
        private bool currentUserIsGameOwner;

        private int selectedMission;

        private ObservableList<PlayerData> connectedPlayers;

        public ObservableList<PlayerData> ConnectedPlayers
        {
            get => connectedPlayers;
            set => Set(ref connectedPlayers, value);
        }

        public bool CurrentUserIsGameOwner
        {
            get => currentUserIsGameOwner;
            set => Set(ref currentUserIsGameOwner, value);
        }

        public int SelectedMission
        {
            get => selectedMission;
            set => Set(ref selectedMission, value);
        }
    }
}
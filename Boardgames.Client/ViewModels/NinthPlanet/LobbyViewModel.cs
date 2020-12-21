using Boardgames.Common.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class LobbyViewModel : ViewModelBase
    {
        [Obsolete("Used by WPF designer", true)]
        public LobbyViewModel()
            : this(Enumerable.Range(1, 5).Select(x => new PlayerData { Id = x, Name = $"Player {x}" }).ToList(), true, () => { })
        {
        }

        public LobbyViewModel(List<PlayerData> playerData, bool localPlayerIsGameOwner, Action startGameAction)
        {
            PlayerData = new ObservableCollection<PlayerData>(playerData);
            StartGame = new RelayCommand(startGameAction, () => localPlayerIsGameOwner && PlayerData.Count > 1, true);
            PlayerData.CollectionChanged += OnPlayerDataCollectionChanged;
            LocalPlayerIsGameOwner = localPlayerIsGameOwner;
        }

        public ObservableCollection<PlayerData> PlayerData { get; }

        public RelayCommand StartGame { get; }

        public bool LocalPlayerIsGameOwner { get; }

        private void OnPlayerDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StartGame.RaiseCanExecuteChanged();
        }
    }
}
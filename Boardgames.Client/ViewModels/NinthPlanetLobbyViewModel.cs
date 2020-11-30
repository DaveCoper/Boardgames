using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Command;

namespace Boardgames.Client.ViewModels
{
    public class NinthPlanetLobbyViewModel
    {
        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetLobbyViewModel()
            : this(Enumerable.Range(1, 5).Select(x => new PlayerData { Id = x, Name = $"Player {x}" }).ToList(), true)
        {
        }

        public NinthPlanetLobbyViewModel(List<PlayerData> playerData, bool isGameOwner)
        {
            PlayerData = new ObservableCollection<PlayerData>(playerData);
            StartGame = new RelayCommand(() => { }, () => isGameOwner && PlayerData.Count > 1);
            PlayerData.CollectionChanged += OnPlayerDataCollectionChanged;
        }

        public ObservableCollection<PlayerData> PlayerData { get; }

        public RelayCommand StartGame { get; set; }

        private void OnPlayerDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StartGame.RaiseCanExecuteChanged();
        }
    }
}
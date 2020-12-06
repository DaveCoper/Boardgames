using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.ViewModels
{
    public class NinthPlanetRoundViewModel
    {
        private BoardState boardState;

        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetRoundViewModel()
        {

        }

        public NinthPlanetRoundViewModel(
            int currentUserId,
            BoardState boardState,
            Dictionary<int, PlayerData> playerData)
        {
            this.boardState = boardState ?? throw new ArgumentNullException(nameof(boardState));
            this.Hand = new ObservableCollection<Card>(boardState.CardsInHand);
        }

        public int NumberOfPlayers => this.boardState.PlayerStates.Count;

        public ObservableCollection<Card> Hand { get; } 
    }
}
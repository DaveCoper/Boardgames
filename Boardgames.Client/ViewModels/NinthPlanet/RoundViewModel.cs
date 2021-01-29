using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class RoundViewModel : ViewModelBase
    {
        private readonly int gameId;

        private readonly INinthPlanetService ninthPlanetService;

        private RoundState boardState;

        [Obsolete("Used by WPF designer", true)]
        public RoundViewModel()
        {
        }

        public RoundViewModel(
            int gameId,
            int localPlayerId,
            RoundState boardState,
            Dictionary<int, PlayerData> playerData,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger)
            : base(messenger)
        {
            this.gameId = gameId;
            this.boardState = boardState ?? throw new ArgumentNullException(nameof(boardState));
            this.ninthPlanetService = ninthPlanetService ?? throw new ArgumentNullException(nameof(ninthPlanetService));
            var playerStates = boardState.PlayerStates;

            this.LocalPlayer = new LocalPlayerViewModel(
                gameId,
                playerData[localPlayerId],
                playerStates[localPlayerId],
                boardState.CardsInHand,
                this.ninthPlanetService,
                messenger);

            var otherPlayers = playerStates
                .Where(x => x.Key != localPlayerId)
                .Select(x => new RemotePlayerViewModel(gameId, playerData[x.Key], x.Value, messenger))
                .ToList();

            this.OtherPlayers = new ObservableCollection<RemotePlayerViewModel>(otherPlayers);
            this.CurrentTrick = new ObservableCollection<Card>(boardState.CurrentTrick.Values);

            messenger.Register<CardWasPlayed>(this, OnCardWasPlayed);
        }

        public int NumberOfPlayers => this.boardState.PlayerStates.Count;

        public ObservableCollection<RemotePlayerViewModel> OtherPlayers { get; }

        public ObservableCollection<Card> CurrentTrick { get; }

        public LocalPlayerViewModel LocalPlayer { get; }

        public ICommand CardDoubleClickCommand { get; }

        private void OnCardWasPlayed(CardWasPlayed message)
        {
            if (message.GameId == this.gameId)
            {
                this.CurrentTrick.Add(message.Card);
            }
        }
    }
}
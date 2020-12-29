using Boardgames.Client.Services;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class LocalPlayerViewModel : PlayerStateViewModel
    {
        private readonly INinthPlanetService ninthPlanetService;

        public LocalPlayerViewModel(
            int gameId,
            PlayerData playerData,
            PlayerBoardState state,
            IEnumerable<Card> hand,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger)
            : base(gameId, playerData, state, messenger)
        {
            if (hand is null)
            {
                throw new ArgumentNullException(nameof(hand));
            }

            this.Hand = new ObservableCollection<Card>(hand);
            this.FinishedTasks = new ObservableCollection<TaskCard>(state.FinishedTasks);
            this.UnfinishedTasks = new ObservableCollection<TaskCard>(state.UnfinishedTasks);

            this.ninthPlanetService = ninthPlanetService ?? throw new ArgumentNullException(nameof(ninthPlanetService));
        }

        public ObservableCollection<Card> Hand { get; }

        public ObservableCollection<TaskCard> UnfinishedTasks { get; }

        public ObservableCollection<TaskCard> FinishedTasks { get; }

        protected override void OnCardWasPlayed(Card card)
        {
            this.Hand.Remove(card);
        }

        public async Task CommunicateCardAsync(Card card)
        {
            if (this.CommunicatedCard.HasValue)
            {
                return;
            }

            await this.ninthPlanetService.DisplayCardAsync(this.GameId, card, this.CommunicationTokenPosition);
        }

        public async Task CommunicateTokenPositionAsync(CommunicationTokenPosition communicationTokenPosition)
        {
            if (this.CommunicatedCard.HasValue)
            {
                return;
            }

            await this.ninthPlanetService.DisplayCardAsync(this.GameId, this.CommunicatedCard, communicationTokenPosition);
        }

        public async Task PlayCardAsync(Card card)
        {
            if (this.Hand.Contains(card))
            {
                await this.ninthPlanetService.PlayCardAsync(this.GameId, card);
            }
        }
    }
}
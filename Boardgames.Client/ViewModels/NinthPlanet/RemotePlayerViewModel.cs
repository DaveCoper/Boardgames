using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class RemotePlayerViewModel : PlayerStateViewModel
    {
        private int numberOfCards;

        public RemotePlayerViewModel(
            int gameId,
            PlayerData playerData,
            PlayerBoardState state,
            IMessenger messenger)
            : base(gameId, playerData, state, messenger)
        {
            this.NumberOfCards = state.NumberOfCardsInHand;
        }

        public int NumberOfCards
        {
            get => numberOfCards;
            set => Set(ref numberOfCards, value);
        }

        protected override void OnCardWasPlayed(Card card)
        {
            --NumberOfCards;
        }
    }
}
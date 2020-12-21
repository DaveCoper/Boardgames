using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public abstract class PlayerStateViewModel : ViewModelBase
    {
        private readonly PlayerData playerData;
        private CommunicationTokenPosition? communicationTokenPosition;
        private Card? communicatedCard;

        public PlayerStateViewModel(int gameId, PlayerData playerData, PlayerBoardState state, IMessenger messenger)
            : base(messenger)
        {
            if (state is null)
            {
                throw new System.ArgumentNullException(nameof(state));
            }

            this.playerData = playerData ?? throw new System.ArgumentNullException(nameof(playerData));

            this.GameId = gameId;

            this.CommunicatedCard = state.ComunicatedCard;
            this.CommunicationTokenPosition = state.CommunicationTokenPosition;

            messenger.Register<CardWasPlayed>(this, OnCardWasPlayed);
            messenger.Register<PlayerCommunicatedCard>(this, OnCardWasCommunicated);
        }

        private void OnCardWasCommunicated(PlayerCommunicatedCard obj)
        {
            if (obj.GameId == this.GameId && obj.PlayerId == this.PlayerId)
            {
                CommunicationTokenPosition = obj.TokenPosition;
                CommunicatedCard = obj.Card;
            }
        }

        public int GameId { get; }

        public int PlayerId => playerData.Id;

        public string PlayerName => playerData.Name;

        public Card? CommunicatedCard
        {
            get => communicatedCard;
            set => Set(ref communicatedCard, value);
        }

        public CommunicationTokenPosition? CommunicationTokenPosition
        {
            get => communicationTokenPosition;
            set => Set(ref communicationTokenPosition, value);
        }

        protected abstract void OnCardWasPlayed(Card card);

        private void OnCardWasPlayed(CardWasPlayed obj)
        {
            if (obj.GameId == this.GameId && obj.PlayerId == this.PlayerId)
            {
                OnCardWasPlayed(obj.Card);
            }
        }
    }
}
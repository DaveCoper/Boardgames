using Boardgames.Common.Models;
using Boardgames.Common.Observables;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;

namespace Boardgames.NinthPlanet.Client
{
    public class PlayerState : ObservableObject
    {
        private Card displayedCard;

        private CommunicationTokenPosition? communicationTokenPosition;

        private PlayerData playerData;

        private int numberOfCards;

        private ObservableList<Card> takenCards = new ObservableList<Card>();

        private ObservableList<TaskCard> unfinishedTasks = new ObservableList<TaskCard>();

        private ObservableList<TaskCard> finishedTasks = new ObservableList<TaskCard>();

        private bool isOnTurn;

        public Card DisplayedCard
        {
            get => displayedCard;
            set => Set(ref displayedCard, value);
        }

        public CommunicationTokenPosition? CommunicationTokenPosition
        {
            get => communicationTokenPosition;
            set => Set(ref communicationTokenPosition, value);
        }

        public ObservableList<Card> TakenCards
        {
            get => takenCards;
            set => Set(ref takenCards, value);
        }

        public PlayerData PlayerData
        {
            get => playerData;
            set => Set(ref playerData, value);
        }

        public int NumberOfCards
        {
            get => numberOfCards;
            set => Set(ref numberOfCards, value);
        }

        public bool IsOnTurn
        {
            get => isOnTurn;
            set => Set(ref isOnTurn, value);
        }

        public ObservableList<TaskCard> UnfinishedTasks
        {
            get => unfinishedTasks;
            set => Set(ref unfinishedTasks, value);
        }

        public ObservableList<TaskCard> FinishedTasks
        {
            get => finishedTasks;
            set => Set(ref finishedTasks, value);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.Common.Observables;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;

namespace Boardgames.NinthPlanet.Client
{
    public class GameRound : ObservableObject
    {
        private readonly IPlayerDataProvider playerDataProvider;

        private PlayerData captain;

        private CardColor? colorOfCurrentTrick;

        private UserState userState;

        private bool helpIsAvailable;

        private Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>();

        private ObservableList<TrickPlay> currentTrick = new ObservableList<TrickPlay>();

        private ObservableList<TaskCard> availableGoals = new ObservableList<TaskCard>();

        private ObservableList<PlayerState> stateOfAllies = new ObservableList<PlayerState>();

        private PlayerState playerOnTurn;

        private bool userCanPlay;

        public GameRound(IPlayerDataProvider playerDataProvider)
        {
            this.playerDataProvider = playerDataProvider ?? throw new System.ArgumentNullException(nameof(playerDataProvider));
        }

        public ObservableList<TaskCard> AvailableGoals
        {
            get => availableGoals;
            set => Set(ref availableGoals, value);
        }

        public PlayerData Captain
        {
            get => captain;
            set => Set(ref captain, value);
        }

        public ObservableList<TrickPlay> CurrentTrick
        {
            get => currentTrick;
            set => Set(ref currentTrick, value);
        }

        public CardColor? ColorOfCurrentTrick
        {
            get => colorOfCurrentTrick;
            set => Set(ref colorOfCurrentTrick, value);
        }

        public UserState UserState
        {
            get => userState;
            set => Set(ref userState, value);
        }

        public ObservableList<PlayerState> StateOfAllies
        {
            get => stateOfAllies;
            set => Set(ref stateOfAllies, value);
        }

        public bool HelpIsAvailable
        {
            get => helpIsAvailable;
            set => Set(ref helpIsAvailable, value);
        }

        public PlayerState PlayerOnTurn
        {
            get => playerOnTurn;
            set => Set(ref playerOnTurn, value);
        }

        public bool UserCanPlay
        {
            get => userCanPlay;
            set => Set(ref userCanPlay, value);
        }

        public async Task UpdateState(RoundState roundState)
        {
            var userPlayerData = await this.playerDataProvider.GetPlayerDataForCurrentUserAsync();
            var playerData = await this.playerDataProvider.GetPlayerDataAsync(roundState.PlayerStates.Keys);
            var playersDict = playerData.ToDictionary(x => x.Id);

            this.AvailableGoals = new ObservableList<TaskCard>(roundState.AvailableGoals);
            this.Captain = playersDict[roundState.CaptainId];

            var currentTrick = roundState.CurrentTrick.Select(x => new TrickPlay
            {
                Card = x.Value,
                CardPlayer = playersDict[x.Key]
            }).ToList();

            this.CurrentTrick = new ObservableList<TrickPlay>(currentTrick);
            this.HelpIsAvailable = roundState.HelpIsAvailable;
            this.ColorOfCurrentTrick = roundState.ColorOfCurrentTrick;

            var allies = roundState.PlayerStates
                .Where(x => x.Key != userPlayerData.Id)
                .Select(x => new PlayerState
                {
                    PlayerData = playersDict[x.Key],
                    DisplayedCard = x.Value.ComunicatedCard,
                    CommunicationTokenPosition = x.Value.CommunicationTokenPosition,
                    TakenCards = new ObservableList<Card>(x.Value.TakenCards),
                    NumberOfCards = x.Value.NumberOfCardsInHand,
                    UnfinishedTasks = new ObservableList<TaskCard>(x.Value.UnfinishedTasks),
                    FinishedTasks = new ObservableList<TaskCard>(x.Value.FinishedTasks),
                })
                .ToList();

            this.StateOfAllies = new ObservableList<PlayerState>(allies);

            var userState = roundState.PlayerStates[userPlayerData.Id];
            this.UserState = new UserState
            {
                PlayerData = userPlayerData,
                DisplayedCard = userState.ComunicatedCard,
                CommunicationTokenPosition = userState.CommunicationTokenPosition,
                TakenCards = new ObservableList<Card>(userState.TakenCards),
                NumberOfCards = userState.NumberOfCardsInHand,
                UnfinishedTasks = new ObservableList<TaskCard>(userState.UnfinishedTasks),
                FinishedTasks = new ObservableList<TaskCard>(userState.FinishedTasks),
                CardsInHand = new ObservableList<Card>(roundState.CardsInHand),
            };

            this.playerStates = this.StateOfAllies.ToDictionary(x => x.PlayerData.Id);
            this.playerStates.Add(userPlayerData.Id, this.UserState);

            this.ChangePlayerOnTurn(playerStates[roundState.CurrentPlayer]);
        }

        public async Task CardWasPlayedAsync(int playerId, Card card)
        {
            var playerData = await playerDataProvider.GetPlayerDataAsync(playerId);

            this.CurrentTrick.Add(new TrickPlay
            {
                CardPlayer = playerData,
                Card = card
            });

            if (playerId == this.UserState.PlayerData.Id)
            {
                this.UserState.CardsInHand.Remove(card);
            }

            if (!this.ColorOfCurrentTrick.HasValue)
            {
                this.ColorOfCurrentTrick = card.Color;
            }

            var userState = playerStates[playerId];
            userState.NumberOfCards--;
        }

        public void TrickWasFinished(TrickFinished trickFinished)
        {
            this.CurrentTrick.Clear();
            this.ColorOfCurrentTrick = null;
            var winnerState = playerStates[trickFinished.WinnerPlayerId];

            winnerState.TakenCards.AddRange(trickFinished.TakenCards);

            winnerState.FinishedTasks.AddRange(trickFinished.FinishedTasks);
            winnerState.UnfinishedTasks.RemoveRange(trickFinished.FinishedTasks);
        }

        public void TaskWasTaken(int playerId, TaskCard taskCard)
        {
            this.AvailableGoals.Remove(taskCard);
            var playerState = playerStates[playerId];
            playerState.UnfinishedTasks.Add(taskCard);

            this.ChangePlayerOnTurn();
        }

        public void PlayerComunicatedCard(int playerId, Card? card, CommunicationTokenPosition? tokenPosition)
        {
            var playerState = playerStates[playerId];
            playerState.DisplayedCard = card;
            playerState.CommunicationTokenPosition = tokenPosition;
        }

        private void ChangePlayerOnTurn(PlayerState playerOnTurn)
        {
            if (this.PlayerOnTurn != null)
            {
                this.PlayerOnTurn.IsOnTurn = false;

            }

            this.UserCanPlay = playerOnTurn == UserState;
            this.PlayerOnTurn = playerOnTurn;
            playerOnTurn.IsOnTurn = true;
        }
    }
}
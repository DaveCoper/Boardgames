using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Client
{
    public class GameRound
    {
        private readonly IPlayerDataProvider playerDataProvider;

        private Dictionary<int, PlayerState> playerStates;

        public GameRound(IPlayerDataProvider playerDataProvider)
        {
            this.playerDataProvider = playerDataProvider ?? throw new System.ArgumentNullException(nameof(playerDataProvider));
        }

        public List<TaskCard> AvailableGoals { get; set; }

        public PlayerData Captain { get; set; }

        public Dictionary<int, Card> CurrentTrick { get; set; }

        public CardColor? ColorOfCurrentTrick { get; set; }

        public UserState UserState { get; set; }

        public List<PlayerState> StateOfAllies { get; set; }

        public bool HelpIsAvailable { get; set; }

        public async Task UpdateState(RoundState roundState)
        {
            var userPlayerData = await this.playerDataProvider.GetPlayerDataForCurrentUserAsync();
            var otherPlayers = await this.playerDataProvider.GetPlayerDataAsync(roundState.PlayerStates.Keys);
            var otherPlayersDict = otherPlayers.ToDictionary(x => x.Id);

            this.AvailableGoals = roundState.AvailableGoals;
            this.Captain = otherPlayersDict[roundState.CaptainId];
            this.CurrentTrick = roundState.CurrentTrick;
            this.HelpIsAvailable = roundState.HelpIsAvailable;
            this.ColorOfCurrentTrick = roundState.ColorOfCurrentTrick;

            this.StateOfAllies = roundState.PlayerStates
                .Where(x => x.Key != userPlayerData.Id)
                .Select(x => new PlayerState
                {
                    PlayerData = otherPlayersDict[x.Key],
                    DisplayedCard = x.Value.ComunicatedCard,
                    CommunicationTokenPosition = x.Value.CommunicationTokenPosition,
                    TakenCards = x.Value.TakenCards,
                    NumberOfCards = x.Value.NumberOfCardsInHand,
                    UnfinishedTasks = x.Value.UnfinishedTasks,
                    FinishedTasks = x.Value.FinishedTasks,
                })
                .ToList();

            var userState = roundState.PlayerStates[userPlayerData.Id];
            this.UserState = new UserState
            {
                PlayerData = userPlayerData,
                DisplayedCard = userState.ComunicatedCard,
                CommunicationTokenPosition = userState.CommunicationTokenPosition,
                TakenCards = userState.TakenCards,
                CardsInHand = roundState.CardsInHand,
                NumberOfCards = roundState.CardsInHand.Count,
            };

            this.playerStates = this.StateOfAllies.ToDictionary(x => x.PlayerData.Id);
            this.playerStates.Add(userPlayerData.Id, this.UserState);
        }

        public void CardWasPlayed(int playerId, Card card)
        {
            this.CurrentTrick.Add(playerId, card);

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

            // TODO: create collection with RemoveRange accepting IEnumerable for group operations.
            trickFinished.FinishedTasks.ForEach(x => winnerState.UnfinishedTasks.Remove(x));
        }

        public void TrickWasTaken(int playerId, TaskCard taskCard)
        {
            this.AvailableGoals.Remove(taskCard);
            var playerState = playerStates[playerId];
            playerState.UnfinishedTasks.Add(taskCard);
        }

        public void PlayerComunicatedCard(int playerId, Card? card, CommunicationTokenPosition? tokenPosition)
        {
            var playerState = playerStates[playerId];
            playerState.DisplayedCard = card;
            playerState.CommunicationTokenPosition = tokenPosition;
        }
    }
}
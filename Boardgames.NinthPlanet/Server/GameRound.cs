using System;
using System.Collections.Generic;
using System.Linq;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet.Server
{
    public class GameRound
    {
        private readonly int gameId;

        private readonly SavedRoundState roundState;

        private readonly ILogger<GameRound> logger;

        public GameRound(int gameId, SavedRoundState roundState, ILogger<GameRound> logger)
        {
            this.gameId = gameId;
            this.roundState = roundState ?? throw new ArgumentNullException(nameof(roundState));
            this.logger = logger ?? NullLogger<GameRound>.Instance;
        }

        public bool CanPlayCard(int playerId, Card card)
        {
            if (this.roundState.CurrentPlayerId != playerId ||
                !this.roundState.PlayerStates.TryGetValue(playerId, out var state) ||
                !state.CardsInHand.Contains(card))
            {
                return false;
            }

            if (this.roundState.ColorOfCurrentTrick.HasValue)
            {
                var trickColor = this.roundState.ColorOfCurrentTrick.Value;
                return trickColor == card.Color || state.CardsInHand.TrueForAll(x => x.Color != trickColor);
            }

            return true;
        }

        public void PlayCard(int playerId, Card card)
        {
            if (this.roundState.ColorOfCurrentTrick == null)
            {
                this.roundState.ColorOfCurrentTrick = card.Color;
            }

            var state = this.roundState.PlayerStates[playerId];
            state.CardsInHand.Remove(card);
            this.roundState.CurrentTrick.Add(playerId, card);
        }

        public bool TrickIsComplete()
        {
            return this.roundState.PlayerOrder.TrueForAll(x => this.roundState.CurrentTrick.ContainsKey(x));
        }

        public void MoveToNextPlayer()
        {
            var playerIndex = this.roundState.PlayerOrder.IndexOf(this.roundState.CurrentPlayerId);
            playerIndex = (playerIndex + 1) % this.roundState.PlayerOrder.Count;
            this.roundState.CurrentPlayerId = this.roundState.PlayerOrder[playerIndex];
        }

        public int GetTrickWinner()
        {
            var rockets = this.roundState.CurrentTrick.Values.Where(x => x.Color == CardColor.Rocket).ToList();
            if (rockets.Count > 0)
            {
                var highestRocketValue = rockets.Max(x => x.Value);
                return this.roundState.CurrentTrick.First(x => x.Value.Color == CardColor.Rocket && x.Value.Value == highestRocketValue).Key;
            }

            var coloredCards = this.roundState.CurrentTrick.Values.Where(x => x.Color == this.roundState.ColorOfCurrentTrick).ToList();
            if (coloredCards.Count > 0)
            {
                var highestColorValue = coloredCards.Max(x => x.Value);
                return this.roundState.CurrentTrick.First(x => x.Value.Color == this.roundState.ColorOfCurrentTrick && x.Value.Value == highestColorValue).Key;
            }

            throw new Exception();
        }

        public SavedRoundState SaveCurrentState()
        {
            return this.roundState;
            /*
            return new SavedRoundState
            {
                CaptainPlayerId = this.roundState.CaptainPlayerId,
                AvailableGoals = this.roundState.AvailableGoals.ToList(),
                CurrentTrick = this.roundState.CurrentTrick.ToDictionary(x => x.Key, x => x.Value),
                HelpIsAvailable = this.roundState.HelpIsAvailable,
                ColorOfCurrentTrick = this.roundState.ColorOfCurrentTrick,
                CurrentPlayerId = this.roundState.CurrentPlayerId,
                PlayerOrder = this.roundState.PlayerOrder,
                PlayerStates = this.roundState.PlayerStates.ToDictionary(
                          x => x.Key,
                          x => new PlayerPrivateState
                          {
                              CommunicationTokenPosition = x.Value.CommunicationTokenPosition,
                              ComunicatedCard = x.Value.ComunicatedCard,
                              CardsInHand = x.Value.CardsInHand,
                              UnfinishedTasks = x.Value.UnfinishedTasks,
                              FinishedTasks = x.Value.FinishedTasks,
                          }),
            };
            };
            */
        }

        public bool DisplayCard(int playerId, Card? card, CommunicationTokenPosition? tokenPosition)
        {
            if (this.roundState.PlayerStates.TryGetValue(playerId, out var playerPrivateState))
            {
                bool changeOccured = false;

                // you can't change displayed token.
                if (playerPrivateState.DisplayedCardTokenPosition.HasValue && tokenPosition != playerPrivateState.DisplayedCardTokenPosition)
                {
                    logger.LogWarning($"Denied request for change of displayed card position by player {playerId} in game {gameId}. Player already has card position displayed.");
                    throw new Exception("You can't change displayed token.");
                }

                // you can't change displayed card.
                if (playerPrivateState.DisplayedCard.HasValue && card != playerPrivateState.DisplayedCard)
                {
                    logger.LogWarning($"Denied request for change of displayed card by player {playerId} in game {gameId}. Player already has card displayed.");
                    throw new Exception("You can't change displayed card.");
                }

                // null value is ignored.
                if (tokenPosition.HasValue)
                {
                    // player didn't display card token this round.
                    changeOccured = true;
                    playerPrivateState.DisplayedCardTokenPosition = tokenPosition;
                }

                // null value is ignored.
                if (card.HasValue)
                {
                    // player didn't display card this round.
                    changeOccured = true;
                    playerPrivateState.DisplayedCard = card;
                }

                return changeOccured;
            }

            logger.LogWarning($"Denied request for change of displayed card by player {playerId} in game {gameId}. Player is not in the game.");
            throw new Exception("Player state not found!");
        }

        public void GetDisplayedCard(int playerId, out Card? card, out CommunicationTokenPosition? tokenPosition)
        {
            if (this.roundState.PlayerStates.TryGetValue(playerId, out var playerPrivateState))
            {
                card = playerPrivateState.DisplayedCard;
                tokenPosition = playerPrivateState.DisplayedCardTokenPosition;
                return;
            }

            throw new Exception("Player state not found!");
        }

        public RoundState GetGameState(int playerId)
        {
            var state = new RoundState
            {
                CaptainId = this.roundState.CaptainPlayerId,
                AvailableGoals = this.roundState.AvailableGoals.ToList(),
                CurrentTrick = this.roundState.CurrentTrick.ToDictionary(x => x.Key, x => x.Value),
                CurrentPlayer = this.roundState.CurrentPlayerId,
                PlayOrder = this.roundState.PlayerOrder,
                ColorOfCurrentTrick = this.roundState.ColorOfCurrentTrick,
                HelpIsAvailable = true,
                PlayerStates = this.roundState.PlayerStates.ToDictionary(
                          x => x.Key,
                          x => new PlayerBoardState
                          {
                              CommunicationTokenPosition = x.Value.DisplayedCardTokenPosition,
                              ComunicatedCard = x.Value.DisplayedCard,
                              NumberOfCardsInHand = x.Value.CardsInHand.Count,
                              UnfinishedTasks = x.Value.UnfinishedTasks,
                              FinishedTasks = x.Value.FinishedTasks
                          })
            };

            if (this.roundState.PlayerStates.TryGetValue(playerId, out var currentPlayerState))
            {
                state.CardsInHand = currentPlayerState.CardsInHand;
            }

            return state;
        }

        public void GoToNextTrick(int trickWinnerId)
        {
            this.roundState.CurrentTrick.Clear();
            this.roundState.CurrentPlayerId = trickWinnerId;
            this.roundState.ColorOfCurrentTrick = null;
        }

        public List<TaskCard> GetFinishedTasks(int trickWinnerId)
        {
            var finishedTasks = this.roundState.CurrentTrick.Join(
                this.roundState.PlayerStates[trickWinnerId].UnfinishedTasks,
                trick => trick.Value,
                task => task.Card,
                (trick, task) => task)
                .ToList();

            return finishedTasks;
        }

        public List<TaskCard> GetFailedTasks(int trickWinnerId)
        {
            var failedTasks = this.roundState.CurrentTrick.Join(
                this.roundState.PlayerStates
                    .Where(x => x.Key != trickWinnerId)
                    .SelectMany(x => x.Value.UnfinishedTasks),
                trick => trick.Value,
                task => task.Card,
                (trick, task) => task)
                .Distinct()
                .ToList();

            return failedTasks;
        }

        public List<Card> GetCurrentTrickCards()
        {
            return this.roundState.CurrentTrick.Values.ToList();
        }

        public void TakeTaskCard(int playerId, TaskCard task)
        {
            if (this.roundState.PlayerStates.TryGetValue(playerId, out var playerPrivateState))
            {
                if (this.roundState.AvailableGoals.Remove(task))
                {
                    playerPrivateState.UnfinishedTasks.Add(task);
                    return;
                }

                var msg = $"Task card {task} is not available in current round of game {gameId}.";
                logger.LogWarning(msg);
                throw new Exception(msg);
            }

            logger.LogWarning($"Denied request to play card by player {playerId} in game {gameId}. Player is not in the game.");
            throw new Exception("Player state not found!");
        }
    }
}
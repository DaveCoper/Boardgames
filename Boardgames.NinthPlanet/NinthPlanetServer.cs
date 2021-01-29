using System;
using System.Linq;
using Boardgames.Common.Exceptions;
using Boardgames.Common.Extensions;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using Boardgames.NinthPlanet.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet
{
    public class NinthPlanetServer : INinthPlanetServer
    {
        private readonly IGameRoundFactory gameRoundFactory;

        private readonly ILogger<NinthPlanetServer> logger;

        private GameLobby playerLobby;

        private GameRound currentRound;

        private int currentLevel;

        private int concurencyStamp = 1;

        public NinthPlanetServer(
            GameInfo gameInfo,
            GameLobby playerLobby,
            IGameRoundFactory gameRoundFactory,
            ILogger<NinthPlanetServer> logger)
        {
            this.GameInfo = gameInfo ?? throw new ArgumentNullException(nameof(gameInfo));
            this.playerLobby = playerLobby ?? throw new ArgumentNullException(nameof(playerLobby));
            this.gameRoundFactory = gameRoundFactory ?? throw new ArgumentNullException(nameof(gameRoundFactory));
            this.logger = logger ?? NullLogger<NinthPlanetServer>.Instance;
        }

        public NinthPlanetServer(
            GameInfo gameInfo,
            GameLobby gameLobby,
            GameRound gameRound,
            IGameRoundFactory gameRoundFactory,
            ILogger<NinthPlanetServer> logger)
            : this(gameInfo, gameLobby, gameRoundFactory, logger)
        {
            this.currentRound = gameRound;
        }

        public bool GameIsInLobby => currentRound == null;

        public bool RoundInProgress => currentRound != null;

        public int GameId => GameInfo.Id;

        public int GameOwnerId => GameInfo.OwnerId;

        public GameInfo GameInfo { get; }

        public GameState GetGameState(int playerId)
        {
            var state = new GameState
            {
                GameId = this.GameId,
                LobbyState = playerLobby.GetState(),
                ConcurencyStamp = this.concurencyStamp,
                RoundState = null,
            };

            if (!GameIsInLobby)
            {
                state.RoundState = this.currentRound.GetGameState(playerId);
            }

            return state;
        }

        public SavedGameState SaveCurrentState()
        {
            return new SavedGameState
            {
                GameInfo = this.GameInfo,
                PlayersInLobby = this.playerLobby.PlayersInLobby.ToList(),
                RoundState = this.GameIsInLobby ? null : this.currentRound.SaveCurrentState(),
            };
        }

        public void CallForHelp(
            int playerId,
            IGameMessenger gameMessenger)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Displays card in front of player. Each player can display only single card. All additional request will throw exception. For more info check game communication rule in rulebook.
        /// </summary>
        /// <param name="playerId">Player id of player that wants to display card.</param>
        /// <param name="card">Card to display. If set to null parameter is ignored.</param>
        /// <param name="tokenPosition">Card token position. If set to null parameter is ignored.</param>
        /// <param name="gameMessenger">Game messenger used to contact other players.</param>
        public void DisplayCard(
            int playerId,
            Card? card,
            CommunicationTokenPosition? tokenPosition,
            IGameMessenger gameMessenger)
        {
            if (gameMessenger is null)
            {
                throw new ArgumentNullException(nameof(gameMessenger));
            }

            this.ThrowIfPlayerIsNotInGame(playerId);

            if (RoundInProgress)
            {
                var changeOccured = this.currentRound.DisplayCard(playerId, card, tokenPosition);
                if (changeOccured)
                {
                    // notify all players about the change via messenger
                    var msg = this.CreateGameMsg<PlayerCommunicatedCard>();

                    // get current state of player cards.
                    this.currentRound.GetDisplayedCard(playerId, out card, out tokenPosition);
                    msg.Card = card;
                    msg.TokenPosition = tokenPosition;
                    gameMessenger.SendMessage(msg, this.playerLobby.PlayersInLobby);
                }
            }
        }

        /// <summary>
        /// Adds player to the game. Player will have to be spectator until next round starts.
        /// Throws <see cref="GameIsFullException"/> if maximum number of players was reached.
        /// </summary>
        /// <param name="newPlayerId">Player id of new player.</param>
        /// <param name="gameMessenger">Game messenger used to contact other players.</param>
        /// <returns></returns>
        public GameState JoinGame(int newPlayerId, IGameMessenger gameMessenger)
        {
            if (gameMessenger is null)
            {
                throw new ArgumentNullException(nameof(gameMessenger));
            }

            if (this.playerLobby.AddPlayer(newPlayerId))
            {
                var msg = this.CreateGameMsg<NewPlayerConnected>();
                msg.NewPlayerId = newPlayerId;

                gameMessenger.SendMessage(msg, this.playerLobby.PlayersInLobby.Where(x => x != newPlayerId));
                this.playerLobby.AddPlayer(newPlayerId);
            }

            return GetGameState(newPlayerId);
        }

        public void LeaveGame(int playerId, IGameMessenger gameMessenger)
        {
            throw new NotImplementedException();
        }

        public void BeginRound(int userId, IGameMessenger gameMessenger)
        {
            if (RoundInProgress)
                throw new InvalidOperationException("Round is already running.");

            if (userId != GameOwnerId)
                throw new InvalidOperationException("Only game owner can start round.");

            // time based rng will suffice
            var rng = new Random();
            var playersAtTable = this.playerLobby.PlayersInLobby.ToList();
            var playerStates = this.playerLobby.PlayersInLobby.ToDictionary(x => x, x => new PlayerPrivateState());

            var cardDeck = DeckBuilder.CreateCardDeck();
            cardDeck = cardDeck.FisherYatesShuffle(rng);

            int playerIndex = 0;

            // deal cards
            foreach (var card in cardDeck)
            {
                var playerId = playersAtTable[playerIndex];
                playerIndex = (++playerIndex) % playersAtTable.Count;

                var playerState = playerStates[playerId];
                playerState.CardsInHand.Add(card);
            }

            foreach (var playerId in playersAtTable)
            {
                var playerState = playerStates[playerId];
                playerState.CardsInHand.Sort(new CardComparer());
            }

            var goalDeck = DeckBuilder.CreateGoalDeck();
            goalDeck = goalDeck.FisherYatesShuffle(rng);

            var playerOrder = playersAtTable.FisherYatesShuffle(rng);
            var captain = playerOrder[rng.Next(0, playerOrder.Count - 1)];
            this.currentRound = gameRoundFactory.CreateGameRound(
                GameInfo,
                new SavedRoundState
                {
                    CaptainPlayerId = captain,
                    CurrentPlayerId = captain,
                    AvailableGoals = goalDeck.Take(5).Select(x => new TaskCard { Card = x }).ToList(),
                    PlayerStates = playerStates,
                    ColorOfCurrentTrick = null,
                    HelpIsAvailable = true,
                    PlayerOrder = playerOrder
                });

            foreach (var playerId in playersAtTable)
            {
                // each player can see different board state.
                var msg = CreateGameMsg<GameHasStarted>();
                msg.State = this.GetGameState(playerId);
                gameMessenger.SendMessage(msg, playerId);
            };
        }

        public void PlayCard(int playerId, Card card, IGameMessenger gameMessenger)
        {
            this.ThrowIfPlayerIsNotInGame(playerId);
            if (GameIsInLobby)
                throw new InvalidOperationException("Round is not started.");

            if (!this.currentRound.CanPlayCard(playerId, card))
            {
                throw new InvalidOperationException("You can't play this card at current time.");
            }

            this.currentRound.PlayCard(playerId, card);

            var cardPlayedMsg = this.CreateGameMsg<CardWasPlayed>();
            cardPlayedMsg.Card = card;
            cardPlayedMsg.PlayerId = playerId;
            gameMessenger.SendMessage(cardPlayedMsg, this.playerLobby.PlayersInLobby);

            if (this.currentRound.TrickIsComplete())
            {
                var trickWinnerId = this.currentRound.GetTrickWinner();
                var failedGoals = this.currentRound.GetFailedTasks(trickWinnerId);

                if (failedGoals.Count > 0)
                {
                    var roudFailedMsg = this.CreateGameMsg<RoundFailed>();
                    gameMessenger.SendMessage(roudFailedMsg, this.playerLobby.PlayersInLobby);
                    this.currentRound = null;
                    return;
                }

                var finishedTasks = this.currentRound.GetFinishedTasks(trickWinnerId);
                this.currentRound.GoToNextTrick(trickWinnerId);

                var trickMsg = CreateGameMsg<TrickFinished>();
                trickMsg.WinnerPlayerId = trickWinnerId;
                trickMsg.FinishedTasks = finishedTasks;
                trickMsg.TakenCards = this.currentRound.GetCurrentTrickCards();

                gameMessenger.SendMessage(trickMsg, this.playerLobby.PlayersInLobby);
            }
            else
            {
                this.currentRound.MoveToNextPlayer();
            }
        }

        public void TakeTaskCard(int playerId, TaskCard task, IGameMessenger gameMessenger)
        {
            this.ThrowIfPlayerIsNotInGame(playerId);
            if (GameIsInLobby)
                throw new InvalidOperationException("Round is not started.");

            this.currentRound.TakeTaskCard(playerId, task);

            var msg = CreateGameMsg<TaskWasTaken>();
            msg.TaskCard = task;
            msg.PlayerId = playerId;

            gameMessenger.SendMessage(msg, this.playerLobby.PlayersInLobby);
        }

        private TMessageType CreateGameMsg<TMessageType>() where TMessageType : IGameMessage, new()
        {
            var gameMsg = new TMessageType
            {
                GameId = this.GameId,
                ConcurencyStamp = ++concurencyStamp,
            };

            return gameMsg;
        }

        private void ThrowIfPlayerIsNotInGame(int playerId)
        {
            if (!this.playerLobby.Contains(playerId))
            {
                throw new Exception();
            }
        }
    }
}
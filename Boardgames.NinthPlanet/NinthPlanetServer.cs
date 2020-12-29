using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Exceptions;
using Boardgames.Common.Extensions;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    public class NinthPlanetServer : INinthPlanetServer
    {
        private HashSet<int> playersInLobby;

        private Round currentRound;

        private int currentLevel;

        private int concurencyStamp = 1;

        public NinthPlanetServer(GameInfo gameInfo, GameState gameState)
        {
            this.GameInfo = gameInfo;

            if (gameState.BoardState == null)
            {
                CreateLobbyFromState(gameState);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool WaitingInLobby => currentRound == null;

        public int GameId => GameInfo.Id;

        public int GameOwnerId => GameInfo.OwnerId;

        public GameInfo GameInfo { get; }

        public GameState GetGameState(int playerId)
        {
            var state = new GameState
            {
                GameId = this.GameId,
                LobbyState = new LobbyState { ConnectedPlayers = playersInLobby.ToList() }
            };

            if (!WaitingInLobby && this.currentRound.StateOfPlayersAtTable.TryGetValue(playerId, out var currentPlayerState))
            {
                state.BoardState = new BoardState
                {
                    CardsInHand = currentPlayerState.CardsInHand,
                    PlayerStates = this.currentRound.StateOfPlayersAtTable.ToDictionary(
                        x => x.Key,
                        x => new PlayerBoardState
                        {
                            CommunicationTokenPosition = x.Value.CommunicationTokenPosition,
                            ComunicatedCard = x.Value.ComunicatedCard,
                            NumberOfCardsInHand = x.Value.CardsInHand.Count,
                        })
                };
            }

            return state;
        }

        public void CallForHelp(
            int playerId,
            IGameMessenger gameMessenger)
        {
            throw new NotImplementedException();
        }

        public void DisplayCard(
            int playerId,
            Card? card,
            CommunicationTokenPosition? tokenPosition,
            IGameMessenger gameMessenger)
        {
            throw new NotImplementedException();
        }

        public GameState JoinGame(int newPlayerId, IGameMessenger gameMessenger)
        {
            if (gameMessenger is null)
            {
                throw new ArgumentNullException(nameof(gameMessenger));
            }

            if (!this.playersInLobby.Contains(newPlayerId))
            {
                int maximumNumberOfPlayers = GameInfo.MaximumNumberOfPlayers;

                if (this.playersInLobby.Count >= maximumNumberOfPlayers)
                    throw new GameIsFullException(this.GameId, maximumNumberOfPlayers);

                var msg = this.CreateGameMsg<NewPlayerConnected>();
                msg.NewPlayerId = newPlayerId;

                gameMessenger.SendMessage(msg, this.playersInLobby);
                this.playersInLobby.Add(newPlayerId);
            }

            return GetGameState(newPlayerId);
        }

        public void LeaveGame(int playerId, IGameMessenger gameMessenger)
        {
            if (gameMessenger is null)
            {
                throw new ArgumentNullException(nameof(gameMessenger));
            }

            if (this.playersInLobby.Remove(playerId))
            {
                bool roundIsLost = !WaitingInLobby && this.currentRound.StateOfPlayersAtTable.Remove(playerId);

                var msg = this.CreateGameMsg<PlayerHasLeft>();
                msg.PlayerId = playerId;

                gameMessenger.SendMessage(msg, playersInLobby);

                if (roundIsLost)
                {
                    var roundLostMsg = this.CreateGameMsg<RoundFailed>();
                    gameMessenger.SendMessage(roundLostMsg, playersInLobby);
                }
            }
        }

        public void BeginRound(int userId, IGameMessenger gameMessenger)
        {
            if (!WaitingInLobby)
                throw new InvalidOperationException("Round is already running");

            var rng = new Random();

            var playersAtTable = this.playersInLobby.ToList();
            var playerStates = this.playersInLobby.ToDictionary(x => x, x => new PlayerPrivateState());

            var cardDeck = CreateCardDeck();
            cardDeck = cardDeck.FisherYatesShuffle(rng);

            int playerIndex = 0;

            // deal cards
            foreach (var card in cardDeck)
            {
                var playerId = playersAtTable[playerIndex];
                playerIndex = (++playerIndex) % this.playersInLobby.Count;

                var playerState = playerStates[playerId];
                playerState.CardsInHand.Add(card);
            }

            var goalDeck = CreateGoalDeck();
            goalDeck = goalDeck.FisherYatesShuffle(rng);

            this.currentRound = new Round
            {
                AvailableGoals = goalDeck.Take(5).Select(x => new TaskCard { Card = x }).ToList(),
                StateOfPlayersAtTable = playerStates,
                PlayerOrder = playerStates.Keys.ToList(),
            };

            foreach (var player in playersAtTable)
            {
                // each player can see different board state.
                var msg = new GameHasStarted { State = this.GetGameState(player) };
                gameMessenger.SendMessage(msg, player);
            };
        }

        public void PlayCard(int playerId, Card card, IGameMessenger gameMessenger)
        {
            if (WaitingInLobby)
            {
                return;
            }

            this.ThrowIfPlayerIsNotInGame(playerId);
            if (!this.currentRound.CanPlayCard(playerId, card))
            {
                throw new InvalidOperationException("You can't play this card at current time.");
            }

            this.currentRound.PlayCard(playerId, card);
            var msg = this.CreateGameMsg<CardWasPlayed>();
            msg.Card = card;
            gameMessenger.SendMessage(msg, this.playersInLobby);

            if (this.currentRound.TrickIsComplete())
            {
                var trickWinnerId = this.currentRound.GetTrickWinner();
                var finishedGoals = this.currentRound.GetFinishedTasks(trickWinnerId);
                var failedGoals = this.currentRound.GetFailedTasks(trickWinnerId);


                this.currentRound.GoToNextTrick(trickWinnerId);

                var trickMsg = CreateGameMsg<TrickFinished>();
                trickMsg.WinnerPlayerId = trickWinnerId;
                trickMsg.TakenCards = this.currentRound.CurrentTrick.Values.ToList();

                gameMessenger.SendMessage(trickMsg, this.playersInLobby);


            }
            else
            {
                this.currentRound.MoveToNextPlayer();
            }            
        }

        public void TakeGoal(int playerId, TaskCard goal, IGameMessenger gameMessenger)
        {

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

        private List<Card> CreateGoalDeck()
        {
            var deck = CreateGoalDeck();

            // add rocket cards
            for (int i = 1; i <= 4; ++i)
            {
                deck.Add(new Card { Value = i, Color = CardColor.Rocket });
            }

            return deck;
        }

        private List<Card> CreateCardDeck()
        {
            // 9 cards 4 colors
            List<Card> deck = new List<Card>(9 * 4);
            for (int i = 1; i <= 9; ++i)
            {
                deck.Add(new Card { Value = i, Color = CardColor.Blue });
                deck.Add(new Card { Value = i, Color = CardColor.Green });
                deck.Add(new Card { Value = i, Color = CardColor.Pink });
                deck.Add(new Card { Value = i, Color = CardColor.Yellow });
            }

            return deck;
        }

        private void CreateLobbyFromState(GameState gameState)
        {
            if (gameState.LobbyState != null)
            {
                playersInLobby = new HashSet<int>(gameState.LobbyState.ConnectedPlayers);
            }
            else
            {
                playersInLobby = new HashSet<int>();
            }

            playersInLobby.Add(this.GameOwnerId);
        }

        private void ThrowIfPlayerIsNotInGame(int playerId)
        {
            if (!PlayerIsInGame(playerId))
            {
                throw new Exception();
            }
        }

        private bool PlayerIsInGame(int playerId)
        {
            return this.playersInLobby.Contains(playerId);
        }
    }
}
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
        public HashSet<int> playersInLobby;

        public Dictionary<int, PlayerPrivateState> stateOfPlayersAtTable;

        public int currentLevel;

        public NinthPlanetServer(GameInfo gameInfo, GameState gameState)
        {
            this.GameInfo = gameInfo;

            CreateLobbyFromState(gameState);
            WaitingInLobby = gameState.BoardState == null;
        }

        public bool WaitingInLobby { get; private set; }

        public int GameId => GameInfo.Id;

        public int GameOwnerId => GameInfo.OwnerId;

        public GameInfo GameInfo { get; }

        public Task<GameState> GetGameStateAsync(int playerId)
        {
            var state = new GameState
            {
                GameId = this.GameId,
                LobbyState = new LobbyState { ConnectedPlayers = playersInLobby.ToList() }
            };

            if (!WaitingInLobby)
            {
                state.BoardState = new BoardState
                {
                    PlayerStates = this.stateOfPlayersAtTable.ToDictionary(
                        x => x.Key,
                        x => new PlayerBoardState
                        {
                            ComunicationTokenPosition = x.Value.ComunicationTokenPosition,
                            ComunicatedCard = x.Value.ComunicatedCard,
                            NumberOfCardsInHand = x.Value.CardsInHands.Count,
                        })
                };
            }

            return Task.FromResult(state);
        }

        public Task CallForHelpAsync(
            int playerId,
            Queue<GameMessage> messageQueue)
        {
            throw new NotImplementedException();
        }

        public Task DisplayCardAsync(
            int playerId,
            Card card,
            ComunicationTokenPosition? tokenPosition,
            Queue<GameMessage> messageQueue)
        {
            throw new NotImplementedException();
        }

        public async Task<GameState> JoinGameAsync(int newPlayerId, Queue<GameMessage> messageQueue)
        {
            if (messageQueue is null)
            {
                throw new ArgumentNullException(nameof(messageQueue));
            }

            if (!this.playersInLobby.Contains(newPlayerId))
            {
                int maximumNumberOfPlayers = GameInfo.MaximumNumberOfPlayers;

                if (this.playersInLobby.Count >= maximumNumberOfPlayers)
                    throw new GameIsFullException(this.GameId, maximumNumberOfPlayers);

                var msg = new NewPlayerConnected
                {
                    GameId = this.GameId,
                    NewPlayerId = newPlayerId
                };

                foreach (var playerInLobby in this.playersInLobby)
                {
                    messageQueue.Enqueue(CreateMessage(playerInLobby, msg));
                }

                this.playersInLobby.Add(newPlayerId);
            }

            return await GetGameStateAsync(newPlayerId);
        }

        public Task LeaveGameAsync(int playerId, Queue<GameMessage> messageQueue)
        {
            if (messageQueue is null)
            {
                throw new ArgumentNullException(nameof(messageQueue));
            }

            if (this.playersInLobby.Remove(playerId))
            {
                bool roundIsLost = !WaitingInLobby && this.stateOfPlayersAtTable.Remove(playerId);
                var roundLostMsg = new RoundFailed();
                var msg = new PlayerHasLeft
                {
                    GameId = this.GameId,
                    PlayerId = playerId
                };

                foreach (var playerInLobby in playersInLobby)
                {
                    messageQueue.Enqueue(CreateMessage(playerInLobby, msg));

                    if (roundIsLost)
                    {
                        messageQueue.Enqueue(CreateMessage(playerInLobby, roundLostMsg));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task BeginRoundAsync(Queue<GameMessage> messageQueue)
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
                playerState.CardsInHands.Add(card);
            }

            var goalDeck = CreateGoalDeck();
            goalDeck = goalDeck.FisherYatesShuffle(rng);

            return Task.CompletedTask;
        }

        public Task PlayCardAsync(int playerId, Card card, Queue<GameMessage> messageQueue)
        {
            throw new NotImplementedException();
        }

        public Task TakeGoalAsync(int playerId, TaskCard goal, Queue<GameMessage> messageQueue)
        {
            throw new NotImplementedException();
        }

        private GameMessage CreateMessage(int receiverId, object payload)
        {
            return new GameMessage
            {
                ReceiverId = receiverId,
                Payload = payload,
                GameId = this.GameId
            };
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
    }
}
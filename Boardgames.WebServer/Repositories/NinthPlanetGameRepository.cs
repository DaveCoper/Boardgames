using Boardgames.Client.Models;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Boardgames.NinthPlanet.Server;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Models;
using Boardgames.WebServer.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.WebServer.Repositories
{
    public class NinthPlanetGameRepository : INinthPlanetGameRepository
    {
        private readonly IGameServerFactory gameServerFactory;

        private readonly IGameCache<Games.NinthPlanet> gameCache;

        private readonly ApplicationDbContext dbContext;

        private readonly SemaphoreSlim semaphore;

        public NinthPlanetGameRepository(
            IGameServerFactory gameServerFactory,
            IGameCache<Games.NinthPlanet> gameCache,
            ApplicationDbContext dbContext)
        {
            semaphore = new SemaphoreSlim(1);

            this.gameServerFactory = gameServerFactory ?? throw new System.ArgumentNullException(nameof(gameServerFactory));
            this.gameCache = gameCache ?? throw new System.ArgumentNullException(nameof(gameCache));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
        }

        public async Task<Games.NinthPlanet> CreateNewGameAsync(
            int ownerId,
            NinthPlanetNewGameOptions newGameOptions,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var gameInfo = new DbGameInfo
                {
                    GameType = GameType.NinthPlanet,
                    Name = newGameOptions.Name,
                    MaximumNumberOfPlayers = newGameOptions.MaxNumberOfPlayers,
                    IsPublic = newGameOptions.IsPublic,
                    OwnerId = ownerId
                };

                var gameServer = gameServerFactory.CreateNewGame(gameInfo);
                var state = gameServer.SaveCurrentState();
                NinthPlanetGameState dbGameState = ToStoredState(gameInfo, state);
                dbContext.NinthPlanetGames.Add(dbGameState);
                await dbContext.SaveChangesAsync(cancellationToken);

                var game = new Games.NinthPlanet(gameServer);
                this.gameCache.TryAddGame(gameInfo.Id, game);
                return game;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<Games.NinthPlanet> GetGameAsync(int gameId, CancellationToken cancellationToken)
        {
            if (this.gameCache.TryGetGame(gameId, out var game))
                return game;

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                // try again, this will handle cases when two or more requests are looking for a same non-cached game
                if (this.gameCache.TryGetGame(gameId, out game))
                    return game;

                var gameState = await dbContext.NinthPlanetGames
                    .Include(x => x.GameInfo)
                    .Include(x => x.PlayerStates)
                    .FirstAsync(x => gameId == x.GameId, cancellationToken);

                if (gameState == null)
                    throw new GameNotFoundException(gameId);

                return StartGameFromState(gameState);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task SaveRunningGames(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var games = this.gameCache.ToList();

                const int stepSize = 20;
                int position = 0;

                while (position < games.Count)
                {
                    var processedGames = games
                        .Skip(position)
                        .Take(stepSize)
                        .ToList();

                    var processedGameIds = processedGames
                        .Select(x => x.GameId)
                        .ToList();

                    var stroredGames = await dbContext.NinthPlanetGames
                        .Where(x => processedGameIds.Contains(x.GameId))
                        .ToDictionaryAsync(x => x.GameId);

                    foreach (var game in processedGames)
                    {
                        var state = await game.SaveCurrentStateAsync();
                        throw new NotImplementedException();
                    }

                    await dbContext.SaveChangesAsync();
                    position += stepSize;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private NinthPlanetGameState ToStoredState(DbGameInfo gameInfo, SavedGameState state)
        {
            var playerStates = state.PlayersInLobby
                    .Select(x => new NinthPlanetPlayerState { PlayerId = x })
                    .ToList();

            var dbGameState = new NinthPlanetGameState
            {
                GameInfo = gameInfo,
                RoundIsInProgress = state.RoundState != null,
                PlayerStates = playerStates,
            };

            if (state.RoundState != null)
            {
                foreach (var playerState in state.RoundState.PlayerStates)
                {
                    var storedState = playerStates.First(x => x.PlayerId == playerState.Key);
                    storedState.PlayOrder = state.RoundState.PlayerOrder.IndexOf(playerState.Key);

                    storedState.FinishedTasks = playerState.Value.FinishedTasks;
                    storedState.UnfinishedTasks = playerState.Value.UnfinishedTasks;
                    storedState.CardsInHand = playerState.Value.CardsInHand;
                    storedState.ComunicatedCardColor = playerState.Value.DisplayedCard.Color;
                    storedState.ComunicatedCardValue = playerState.Value.DisplayedCard.Value;
                    storedState.CommunicationTokenPosition = playerState.Value.DisplayedCardTokenPosition;
                }
            }

            return dbGameState;
        }

        private SavedGameState ToGameState(NinthPlanetGameState dbGameState)
        {
            var gameState = new SavedGameState
            {
                GameInfo = dbGameState.GameInfo,
                PlayersInLobby = dbGameState.PlayerStates.Select(x => x.PlayerId).ToList(),
            };

            if (dbGameState.RoundIsInProgress)
            {
                var playerOrder = dbGameState.PlayerStates
                    .Where(x => x.PlayOrder.HasValue)
                    .OrderBy(x => x.PlayOrder.Value)
                    .Select(x => x.PlayerId)
                    .ToList();

                var playerStates = dbGameState.PlayerStates
                    .Where(x => x.PlayOrder.HasValue)
                    .ToDictionary(
                        x => x.PlayerId,
                        x => new PlayerPrivateState
                        {
                            CardsInHand = x.CardsInHand,
                            DisplayedCard = x.ComunicatedCardValue.HasValue ? new Card
                            {
                                Value = x.ComunicatedCardValue.Value,
                                Color = x.ComunicatedCardColor.Value
                            } : null,
                            DisplayedCardTokenPosition = x.CommunicationTokenPosition,
                            UnfinishedTasks = x.UnfinishedTasks,
                            FinishedTasks = x.FinishedTasks,
                        });

                gameState.RoundState = new SavedRoundState
                {
                    PlayerOrder = playerOrder,
                    CurrentPlayerId = dbGameState.CurrentPlayerId,
                    CaptainPlayerId = dbGameState.CaptainPlayerId,
                    ColorOfCurrentTrick = dbGameState.ColorOfCurrentTrick,
                    PlayerStates = playerStates,
                };
            }

            return gameState;
        }

        private Games.NinthPlanet StartGameFromState(NinthPlanetGameState dbGameState)
        {
            SavedGameState gameState = ToGameState(dbGameState);

            var gameServer = gameServerFactory.CreateGameFromState(gameState);
            var game = new Games.NinthPlanet(gameServer);
            this.gameCache.TryAddGame(dbGameState.GameId, game);
            return game;
        }
    }
}
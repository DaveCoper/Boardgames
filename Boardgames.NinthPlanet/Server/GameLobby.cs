using System.Collections.Generic;
using System.Linq;
using Boardgames.Common.Exceptions;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet.Server
{
    public class GameLobby
    {
        private readonly int gameId;

        private readonly ILogger<GameLobby> logger;

        private readonly int capacity;

        public GameLobby(
            int gameId,
            int capacity,
            ILogger<GameLobby> logger)
            : this(gameId, capacity, null, logger)
        {
        }

        public GameLobby(
            int gameId,
            int capacity,
            IEnumerable<int> playersInLobby,
            ILogger<GameLobby> logger)
        {
            this.gameId = gameId;
            this.capacity = capacity;
            this.logger = logger ?? NullLogger<GameLobby>.Instance;

            if (playersInLobby == null)
            {
                this.PlayersInLobby = new HashSet<int>();
            }
            else
            {
                this.PlayersInLobby = new HashSet<int>(playersInLobby);
            }
        }

        public bool GameIsFull => this.capacity <= this.PlayersInLobby.Count;

        public HashSet<int> PlayersInLobby { get; }

        public bool AddPlayer(int playerId)
        {
            if (!this.Contains(playerId))
            {
                if (this.GameIsFull)
                    throw new GameIsFullException(this.gameId, this.capacity);

                if (PlayersInLobby.Add(playerId))
                {
                    logger.LogInformation($"Player {playerId} joined game {gameId}.");
                    return true;
                }
            }

            return false;
        }

        public LobbyState GetState()
        {
            return new LobbyState
            {
                ConnectedPlayers = PlayersInLobby.ToList()
            };
        }

        public bool RemovePlayer(int playerId)
        {
            if (PlayersInLobby.Remove(playerId))
            {
                logger.LogInformation($"Player {playerId} left game {gameId}.");
                return true;
            }

            return false;
        }

        public bool Contains(int playerId)
        {
            return PlayersInLobby.Contains(playerId);
        }
    }
}
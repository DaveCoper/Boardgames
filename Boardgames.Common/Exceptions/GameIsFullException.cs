using System;
using System.Runtime.Serialization;

namespace Boardgames.Common.Exceptions
{
    [Serializable]
    public class GameIsFullException : InvalidOperationException
    {
        public GameIsFullException(int gameId, int maxNumberOfPlayers) : base(FormatMessage(gameId, maxNumberOfPlayers))
        {
            GameId = gameId;
            MaxNumberOfPlayers = maxNumberOfPlayers;
        }

        public GameIsFullException(int gameId, int maxNumberOfPlayers, Exception innerException) : base(FormatMessage(gameId, maxNumberOfPlayers), innerException)
        {
            GameId = gameId;
            MaxNumberOfPlayers = maxNumberOfPlayers;
        }

        protected GameIsFullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            GameId = info.GetInt32(nameof(GameId));
            MaxNumberOfPlayers = info.GetInt32(nameof(MaxNumberOfPlayers));
        }

        public int GameId { get; }

        public int MaxNumberOfPlayers { get; }

        private static string FormatMessage(int gameId, int maxNumberOfPlayers)
        {
            return $"Game {gameId} is full. Maximum number of players is set to {maxNumberOfPlayers}";
        }
    }
}
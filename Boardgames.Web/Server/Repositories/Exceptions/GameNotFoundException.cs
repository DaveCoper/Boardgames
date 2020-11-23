using System;
using System.Runtime.Serialization;

namespace Boardgames.Web.Server.Repositories.Exceptions
{
    [Serializable]
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(int gameId) 
            : base(FormatMessage(gameId))
        {
            GameId = gameId;
        }

        public GameNotFoundException(
            int gameId,
            Exception innerException)
            : base(FormatMessage(gameId), innerException)
        {
            GameId = gameId;
        }

        protected GameNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            GameId = info.GetInt32(nameof(GameId));
        }

        public int GameId { get; }

        private static string FormatMessage(int gameId)
        {
            return $"Game with id {gameId} was not found";
        }
    }
}
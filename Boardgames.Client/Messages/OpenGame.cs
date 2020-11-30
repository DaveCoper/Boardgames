using Boardgames.Common.Models;

namespace Boardgames.Client.Messages
{
    public class OpenGame
    {
        public int GameOwnerId { get; set; }

        public int GameId { get; set; }

        public GameType GameType { get; set; }

        public object GameState { get; set; }
    }
}
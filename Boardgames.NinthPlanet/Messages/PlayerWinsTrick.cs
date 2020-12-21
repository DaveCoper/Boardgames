using Boardgames.Common.Messages;

namespace Boardgames.NinthPlanet.Messages
{
    public class PlayerWinsTrick : GameMessage
    {
        public int WinnerPlayerId { get; set; }
    }
}
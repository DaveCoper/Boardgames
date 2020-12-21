using Boardgames.Common.Messages;

namespace Boardgames.NinthPlanet.Messages
{
    public class NewPlayerConnected : GameMessage
    {
        public int NewPlayerId { get; set; }
    }
}
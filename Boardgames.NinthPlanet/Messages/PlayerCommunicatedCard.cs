using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class PlayerCommunicatedCard : GameMessage
    {
        public int PlayerId { get; set; }

        public Card? Card { get; set; }

        public CommunicationTokenPosition? TokenPosition { get; set; }
    }
}
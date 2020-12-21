using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class PlayerCommunicatedCard
    {
        public int GameId { get; set; }

        public int PlayerId { get; set; }

        public Card? Card { get; set; }

        public CommunicationTokenPosition? TokenPosition { get; set; }
    }
}
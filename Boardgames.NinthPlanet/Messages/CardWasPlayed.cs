using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class CardWasPlayed : GameMessage
    {
        public Card Card { get; set; }

        public int PlayerId { get; set; }
    }
}
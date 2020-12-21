using Boardgames.Common.Messages;

namespace Boardgames.NinthPlanet.Messages
{
    public class PlayerHasLeft : GameMessage
    {    
        public int PlayerId { get; set; }
    }
}
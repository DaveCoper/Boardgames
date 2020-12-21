using System.Collections.Generic;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class TrickFinished : GameMessage
    {
        public int WinnerPlayerId { get; set; }

        public List<Card> TakenCards { get; set; }
    }
}
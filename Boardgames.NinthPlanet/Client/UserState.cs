using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Client
{
    public class UserState : PlayerState
    {
        public List<Card> CardsInHand { get; set; }
    }
}
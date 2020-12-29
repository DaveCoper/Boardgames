using Boardgames.NinthPlanet.Models;
using System.Collections.Generic;

namespace Boardgames.NinthPlanet
{
    internal class CardComparer : IComparer<Card>
    {
        public int Compare(Card c1, Card c2)
        {
            if (c1.Color < c2.Color)
                return -1;

            if (c1.Color > c2.Color)
                return 1;

            return c1.Value - c2.Value;
        }
    }
}
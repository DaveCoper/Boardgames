using System;
using System.Collections.Generic;
using System.Text;

namespace Boardgames.NinthPlanet.Models
{

    public struct TaskCard
    {
        public Card Card { get; set; }

        public TaskCardModifier? Modifier { get; set; }

        public override string ToString()
        {
            if (Modifier.HasValue)
            {
                return $"{Modifier.Value} {Card}";
            }

            return Card.ToString();
        }
    }
}

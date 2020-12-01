using System;

namespace Boardgames.Common.Models
{
    public class GameInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public GameType GameType { get; set; }

        public bool IsPublic { get; set; }

        // Owner of the game
        public int OwnerId { get; set; }

        public int MaximumNumberOfPlayers { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
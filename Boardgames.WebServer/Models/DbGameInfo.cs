using System;
using Boardgames.Common.Models;

namespace Boardgames.WebServer.Models
{
    public class DbGameInfo : GameInfo, IEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}
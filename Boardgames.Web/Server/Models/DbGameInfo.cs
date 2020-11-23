using System;
using Boardgames.Game.Models;

namespace Boardgames.Web.Server.Models
{
    public class DbGameInfo : GameInfo, IEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}
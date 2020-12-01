using System;
using Boardgames.Common.Models;

namespace Boardgames.WebServer.Models
{
    public class DbGameInfo : GameInfo, IEntity
    {
        public ApplicationUser Owner { get; set; }
    }
}
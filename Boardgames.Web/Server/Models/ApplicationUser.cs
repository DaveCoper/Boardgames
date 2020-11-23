using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Game.Models;
using Microsoft.AspNetCore.Identity;

namespace Boardgames.Web.Server.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Avatar { get; set; }

        public ICollection<DbGameInfo> CreatedGames { get; set; } = new List<DbGameInfo>();
}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Boardgames.WebServer.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Avatar { get; set; }

        public ICollection<DbGameInfo> CreatedGames { get; set; } = new List<DbGameInfo>();
}
}

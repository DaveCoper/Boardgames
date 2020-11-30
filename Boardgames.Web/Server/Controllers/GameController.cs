using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boardgames.Game.Models;
using Boardgames.Shared.Models;
using Boardgames.Web.Server.Data;
using Boardgames.Web.Server.Extensions;
using Boardgames.Web.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Boardgames.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public GameController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet()]
        public async Task<List<GameInfo>> GetAsync([FromQuery]int numberOfEntries = 50)
        {
            numberOfEntries = Math.Max(numberOfEntries, 200);

            dbContext.Games
                .OrderBy(x=> x.Id)
                .Select(x=> new GameInfo
                {
                    Id = x.Id,
                    GameType = x.GameType
                })
                .Take(numberOfEntries)
        }
    }
}
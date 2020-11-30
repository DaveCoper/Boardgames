using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.Client.Models;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Extensions;
using Boardgames.WebServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.WebServer.Controllers
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

            var games = await dbContext.Games
                .OrderBy(x => x.Id)
                .Select(x => new GameInfo
                {
                    Id = x.Id,
                    GameType = x.GameType
                })
                .Take(numberOfEntries)
                .ToListAsync();

            return games;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.WebServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public GamesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet()]
        public async Task<List<GameInfo>> GetAsync([FromQuery] int numberOfEntries = 50)
        {
            if (numberOfEntries < 1)
                return new List<GameInfo>();

            numberOfEntries = Math.Min(numberOfEntries, 200);

            var games = await dbContext.Games
                .OrderBy(x => x.Id)
                .Where(x => x.IsPublic)
                .Select(x => new GameInfo
                {
                    Id = x.Id,
                    GameType = x.GameType,
                    IsPublic = x.IsPublic,
                    Name = x.Name,
                    OwnerId = x.OwnerId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    MaximumNumberOfPlayers = x.MaximumNumberOfPlayers
                })
                .Take(numberOfEntries)
                .ToListAsync();

            return games;
        }

        [HttpGet("{gameId}")]
        public async Task<GameInfo> GetByKeyAsync([FromRoute] int gameId)
        {
            var game = await dbContext.Games
                .OrderBy(x => x.Id)
                .Where(x => x.IsPublic)
                .Select(x => new GameInfo
                {
                    Id = x.Id,
                    GameType = x.GameType,
                    IsPublic = x.IsPublic,
                    Name = x.Name,
                    OwnerId = x.OwnerId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    MaximumNumberOfPlayers = x.MaximumNumberOfPlayers
                })
                .FirstOrDefaultAsync();

            return game;
        }
    }
}
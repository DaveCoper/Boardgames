using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public GamesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet()]
        public async Task<List<GameInfo>> GetAsync(
            [FromQuery] int page = 0,
            [FromQuery] int sizeOfPage = 50)
        {
            if (sizeOfPage < 1)
                return new List<GameInfo>();

            sizeOfPage = Math.Min(sizeOfPage, 200);

            var gamesQuery = dbContext.Games
                .OrderBy(x => x.Id)
                .Where(x => x.IsPublic)
                .OrderBy(x => x.Id);

            var games = await gamesQuery
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
                .Skip(page * sizeOfPage)
                .Take(sizeOfPage)
                .ToListAsync();

            return games;
        }

        [HttpGet("Count")]
        public async Task<int> GetNumberOfGamesAsync()
        {
            var numberOfGames = await dbContext.Games
                .OrderBy(x => x.Id)
                .Where(x => x.IsPublic)
                .CountAsync();

            return numberOfGames;
        }

        [HttpGet("{gameId}")]
        public async Task<GameInfo> GetByKeyAsync([FromRoute] int gameId)
        {
            var game = await dbContext.Games
                .OrderBy(x => x.Id)
                .Where(x => x.IsPublic && x.Id == gameId)
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
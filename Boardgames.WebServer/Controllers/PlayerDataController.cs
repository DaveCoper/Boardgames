using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.WebServer.Extensions;
using Boardgames.WebServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerDataController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public PlayerDataController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet()]
        public async Task<PlayerData> GetAsync()
        {
            //var userId = userManager.GetUserId(this.User);

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return await this.userManager.Users.GetPlayerDataAsync(result);
            }

            throw new InvalidOperationException("Used id was in invalid format!");
        }

        [HttpGet("{playerId}")]
        public async Task<PlayerData> GetForIdAsync([FromRoute] int playerId)
        {
            return await userManager.Users.GetPlayerDataAsync(playerId);
        }

        [HttpPost()]
        public async Task<List<PlayerData>> PostAsync([FromBody] List<int> playerIds)
        {
            return await userManager.Users.GetPlayerDataAsync(playerIds);
        }
    }
}
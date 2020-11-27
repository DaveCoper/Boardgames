﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Shared.Models;
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
            var userId = userManager.GetUserId(this.User);
            if (int.TryParse(userId, out var result))
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
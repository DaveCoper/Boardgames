using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.WebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.WebServer.Extensions
{
    public static class UserQueryExtensions
    {
        public static async Task<PlayerData> GetPlayerDataAsync(this IQueryable<ApplicationUser> applicationUsers, int userId)
        {
            var user = await applicationUsers
                .Select(x => new { x.Id, x.UserName, x.Avatar })
                .FirstAsync(x => x.Id == userId);

            var playerData = new PlayerData
            {
                Id = user.Id,
                Name = user.UserName,
                AvatarUri = user.Avatar
            };

            return playerData;
        }

        public static async Task<List<PlayerData>> GetPlayerDataAsync(this IQueryable<ApplicationUser> applicationUsers, IEnumerable<int> userIds)
        {
            var userIdList = userIds as List<int> ?? userIds.ToList();
            var users = await applicationUsers
                .Select(x => new { x.Id, x.UserName, x.Avatar })
                .Where(x => userIdList.Contains(x.Id))
                .ToListAsync();

            var playerDataList = new List<PlayerData>(users.Count);
            foreach (var user in users)
            {
                var playerData = new PlayerData
                {
                    Id = user.Id,
                    Name = user.UserName,
                    AvatarUri = user.Avatar
                };

                playerDataList.Add(playerData);
            }

            return playerDataList;
        }
    }
}
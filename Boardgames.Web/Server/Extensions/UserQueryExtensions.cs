using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Shared.Models;
using Boardgames.Web.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.Web.Server.Extensions
{
    public static class UserQueryExtensions
    {
        public static async Task<PlayerData> GetPlayerDataAsync(this IQueryable<ApplicationUser> applicationUsers, int userId)
        {
            var user = await applicationUsers
                .Select(x => new { x.Id, x.UserName, x.Avatar })
                .FirstAsync(x => x.Id == userId);

            Uri avatarUri;
            if (string.IsNullOrWhiteSpace(user.Avatar) ||
                !Uri.TryCreate(user.Avatar, UriKind.Absolute, out avatarUri))
            {
                avatarUri = null;
            }

            var playerData = new PlayerData
            {
                Id = user.Id,
                Name = user.UserName,
                AvatarUri = avatarUri
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
                Uri avatarUri;
                if (string.IsNullOrWhiteSpace(user.Avatar) ||
                    !Uri.TryCreate(user.Avatar, UriKind.Absolute, out avatarUri))
                {
                    avatarUri = null;
                }

                var playerData = new PlayerData
                {
                    Id = user.Id,
                    Name = user.UserName,
                    AvatarUri = avatarUri
                };

                playerDataList.Add(playerData);
            }

            return playerDataList;
        }
    }
}
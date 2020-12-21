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
                .Select(x => new PlayerData
                {
                    Id = x.Id,
                    Name = x.UserName,
                    AvatarUri = x.Avatar
                })
                .FirstAsync(x => x.Id == userId);

            return user;
        }

        public static async Task<List<PlayerData>> GetPlayerDataAsync(this IQueryable<ApplicationUser> applicationUsers, IEnumerable<int> userIds)
        {
            var userIdList = userIds as List<int> ?? userIds.ToList();
            var users = await applicationUsers
                .Select(x => new PlayerData 
                {
                    Id = x.Id,
                    Name = x.UserName,
                    AvatarUri = x.Avatar
                })
                .Where(x => userIdList.Contains(x.Id))
                .ToListAsync();            

            return users;
        }
    }
}
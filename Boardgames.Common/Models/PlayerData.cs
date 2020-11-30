using System;

namespace Boardgames.Common.Models
{
    public class PlayerData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Uri AvatarUri { get; set; }
    }
}
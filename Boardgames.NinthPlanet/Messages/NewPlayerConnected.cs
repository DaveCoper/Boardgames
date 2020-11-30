using Boardgames.Common.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class NewPlayerConnected
    {
        public int GameId { get; set; }

        public PlayerData NewPlayerData { get; set; }
    }
}
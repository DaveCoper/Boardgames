namespace Boardgames.Web.Server.Models
{
    public class NinthPlanetGameState : NinthPlanet.Models.GameState
    {
        public DbGameInfo GameInfo { get; set; }
    }
}
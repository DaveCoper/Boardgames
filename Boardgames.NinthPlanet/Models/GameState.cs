namespace Boardgames.NinthPlanet.Models
{
    public class GameState
    {
        public int GameId { get; set; }

        public RoundState RoundState { get; set; }

        public LobbyState LobbyState { get; set; }

        public int ConcurencyStamp { get; set; }
    }
}
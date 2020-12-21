namespace Boardgames.Common.Messages
{
    public class GameMessage : IGameMessage
    {
        public int GameId { get; set; }

        public int ConcurencyStamp { get; set; }
    }
}
namespace Boardgames.Common.Messages
{
    public class GameMessage
    {
        public int GameId { get; set; }

        public int ReceiverId { get; set; }

        public object Payload { get; set; }
    }
}
namespace Boardgames.Common.Messages
{
    internal class SignalRGameMessage
    {
        public int ReceiverId { get; set; }

        public IGameMessage Payload { get; set; }
    }
}
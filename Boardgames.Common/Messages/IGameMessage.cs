namespace Boardgames.Common.Messages
{
    public interface IGameMessage
    {
        int ConcurencyStamp { get; set; }
        int GameId { get; set; }
    }
}
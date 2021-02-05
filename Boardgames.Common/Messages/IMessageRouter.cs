using System.Threading.Tasks;

namespace Boardgames.Common.Messages
{
    public interface IMessageRouter
    {
        public int GameId { get; }

        Task RouteMessage(IGameMessage gameMessage);
    }
}

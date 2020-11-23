using System.Threading.Tasks;

namespace Boardgames.Shared.Brookers
{
    public interface IMessageBroker
    {
        Task SubscribeToGameMessagesAsync(int gameId);
    }
}
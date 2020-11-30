using System.Threading.Tasks;

namespace Boardgames.Client.Brookers
{
    public interface IMessageBroker
    {
        Task SubscribeToGameMessagesAsync(int gameId);
    }
}
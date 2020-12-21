using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boardgames.Common.Messages
{
    public interface IGameMessenger
    {
        void SendMessage<TMessageType>(TMessageType message, IEnumerable<int> receiverPlayerIds) where TMessageType : IGameMessage;  

        void SendMessage<TMessageType>(TMessageType message, params int[] receiverPlayerIds) where TMessageType : IGameMessage;
    }
}
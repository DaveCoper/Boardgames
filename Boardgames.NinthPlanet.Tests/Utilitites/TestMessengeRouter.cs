using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Client;

namespace Boardgames.NinthPlanet.Tests.Utilitites
{
    internal class TestMessengeRouter : IGameMessenger
    {
        private readonly Dictionary<int, ClientMessageRouter> gameClients;

        private Queue<Message> messageQueue;

        public TestMessengeRouter()
        {
            this.gameClients = new Dictionary<int, ClientMessageRouter>();
            this.messageQueue = new Queue<Message>();
        }

        public void AddClient(int playerId, NinthPlanetClient client)
        {
            this.gameClients.Add(playerId, new ClientMessageRouter(client));
        }

        public void SendMessage<TMessageType>(TMessageType message, IEnumerable<int> receiverPlayerIds) where TMessageType : IGameMessage
        {
            foreach (var playerId in receiverPlayerIds)
            {
                messageQueue.Enqueue(new Message { Data = message, Receiver = playerId });
            }
        }

        public void SendMessage<TMessageType>(TMessageType message, params int[] receiverPlayerIds) where TMessageType : IGameMessage
        {
            SendMessage(message, (IEnumerable<int>)receiverPlayerIds);
        }

        public async Task FlushAsync()
        {
            while (messageQueue.Count > 0)
            {
                var msg = messageQueue.Dequeue();
                var client = gameClients[msg.Receiver];
                await client.RouteMessage(msg.Data);
            }
        }

        private class Message
        {
            public int Receiver { get; set; }

            public IGameMessage Data { get; set; }
        }
    }
}
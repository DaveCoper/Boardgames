using System;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Messages;

namespace Boardgames.NinthPlanet.Client
{
    public class ClientMessageRouter : IMessageRouter
    {
        public ClientMessageRouter(NinthPlanetClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public NinthPlanetClient Client { get; }

        public int GameId => this.Client.GameId;

        public async Task RouteMessage(IGameMessage gameMessage)
        {
            switch (gameMessage)
            {
                case CardWasPlayed cardWasPlayed:
                    await this.Client.ReceiveMessageAsync(cardWasPlayed);
                    break;

                case SelectedMissionHasChanged selectedMissionHasChanged:
                    await this.Client.ReceiveMessageAsync(selectedMissionHasChanged);
                    break;

                case GameHasStarted gameHasStarted:
                    await this.Client.ReceiveMessageAsync(gameHasStarted);
                    break;

                case NewPlayerConnected newPlayerConnected:
                    await this.Client.ReceiveMessageAsync(newPlayerConnected);
                    break;

                case PlayerCommunicatedCard playerCommunicatedCard:
                    await this.Client.ReceiveMessageAsync(playerCommunicatedCard);
                    break;

                case PlayerHasLeft playerHasLeft:
                    await this.Client.ReceiveMessageAsync(playerHasLeft);
                    break;

                case RoundFailed roundFailed:
                    await this.Client.ReceiveMessageAsync(roundFailed);
                    break;

                case TaskWasTaken taskWasTaken:
                    await this.Client.ReceiveMessageAsync(taskWasTaken);
                    break;

                case TrickFinished trickFinished:
                    await this.Client.ReceiveMessageAsync(trickFinished);
                    break;
            }
        }
    }
}
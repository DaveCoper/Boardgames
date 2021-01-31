using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;
using Newtonsoft.Json;

namespace Boardgames.WebServer.Models
{
    public class NinthPlanetPlayerState
    {
        public NinthPlanetPlayerState()
        {
            this.FinishedTasks = new List<TaskCard>();
            this.UnfinishedTasks = new List<TaskCard>();
            this.CardsInHand = new List<Card>();
        }

        public int? PlayOrder { get; set; }

        public string SerializedCardsInHand
        {
            get => JsonConvert.SerializeObject(this.CardsInHand);
            set => this.CardsInHand = JsonConvert.DeserializeObject<List<Card>>(value);
        }

        public List<Card> CardsInHand { get; set; }

        public string SerializedUnfinishedTasks
        {
            get => JsonConvert.SerializeObject(this.UnfinishedTasks);
            set => this.UnfinishedTasks = JsonConvert.DeserializeObject<List<TaskCard>>(value);
        }

        public List<TaskCard> UnfinishedTasks { get; set; }

        public string SerializedFinishedTasks
        {
            get => JsonConvert.SerializeObject(this.FinishedTasks);
            set => this.FinishedTasks = JsonConvert.DeserializeObject<List<TaskCard>>(value);
        }

        public List<TaskCard> FinishedTasks { get; set; }

        public ApplicationUser Player { get; set; }

        public int PlayerId { get; set; }

        public NinthPlanetGameState GameState { get; set; }

        public int GameId { get; set; }

        public CardColor? ComunicatedCardColor { get; set; }

        public int? ComunicatedCardValue { get; set; }

        public CommunicationTokenPosition? CommunicationTokenPosition { get; set; }
    }
}
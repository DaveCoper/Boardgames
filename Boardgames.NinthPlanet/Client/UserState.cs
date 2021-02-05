using Boardgames.Common.Observables;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Client
{
    public class UserState : PlayerState
    {
        private ObservableList<Card> cardsInHand = new ObservableList<Card>();

        public ObservableList<Card> CardsInHand
        {
            get => cardsInHand;
            set => Set(ref cardsInHand, value);
        }
    }
}
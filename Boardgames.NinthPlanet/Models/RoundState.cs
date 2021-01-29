using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    /// <summary>
    /// Stores state of game board
    /// </summary>
    public class RoundState
    {
        /// <summary>
        /// Id of player that is current captain.
        /// </summary>
        public int CaptainId { get; set; }

        /// <summary>
        /// Indicates if players have available help (satellite symbol)
        /// </summary>
        public bool HelpIsAvailable { get; set; }

        public CardColor? ColorOfCurrentTrick { get; set; }

        public int CurrentPlayer { get; set; }

        public List<int> PlayOrder { get; set; } = new List<int>();

        /// <summary>
        /// List of cards that your player has in hand.
        /// </summary>
        public List<Card> CardsInHand { get; set; } = new List<Card>();

        /// <summary>
        /// Goals that were not taken by the player.
        /// </summary>
        public List<TaskCard> AvailableGoals { get; set; } = new List<TaskCard>();

        /// <summary>
        /// States of boards of individual players (Player Id is used as key)
        /// </summary>
        public Dictionary<int, PlayerBoardState> PlayerStates { get; set; } = new Dictionary<int, PlayerBoardState>();

        /// <summary>
        /// List of cards that are on the played in current trick.
        /// </summary>
        public Dictionary<int, Card> CurrentTrick { get; set; } = new Dictionary<int, Card>();
    }
}
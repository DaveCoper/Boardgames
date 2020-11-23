using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    /// <summary>
    /// Stores state of game board
    /// </summary>
    public class BoardState
    {
        /// <summary>
        /// Id of player that is current captain.
        /// </summary>
        public int CaptainId { get; set; }

        /// <summary>
        /// Goals that were not taken by the player.
        /// </summary>
        public List<Goal> AvailableGoals { get; set; } = new List<Goal>();

        /// <summary>
        /// States of boards of individual players (Player Id is used as key)
        /// </summary>
        public Dictionary<int, PlayerBoardState> PlayerStates { get; set; } = new Dictionary<int, PlayerBoardState>();

        /// <summary>
        /// Indicates if players have available help (satellite symbol)
        /// </summary>
        public bool HelpIsAvailable { get; set; }
    }
}
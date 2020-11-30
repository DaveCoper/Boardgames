using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Web.Server.Games
{
    public class NinthPlanet : INinthPlanetServer
    {
        public NinthPlanet(int gameOwner, GameState gameState)
        {
            this.GameId = gameState.GameId;
            this.GameOwnerId = gameOwner;
        }

        public int GameId { get; }
        public int GameOwnerId { get; }

        public Task CallForHelpAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisplayCardAsync(Card card, TokenPosition? tokenPosition)
        {
            throw new NotImplementedException();
        }

        public Task<GameState> GetGameStateAsync()
        {
            var state = new GameState
            {
                GameId = this.GameId,
                BoardState = null,
                LobbyState = new LobbyState { ConnectedPlayers = new List<int> { this.GameOwnerId } }
            };

            return Task.FromResult(state);
        }

        public Task PlayCardAsync(Card card)
        {
            throw new NotImplementedException();
        }

        public Task TakeGoalAsync(Goal goal)
        {
            throw new NotImplementedException();
        }
    }
}
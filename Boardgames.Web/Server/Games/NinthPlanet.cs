using System;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Web.Server.Games
{
    public class NinthPlanet : INinthPlanetServer
    {
        public NinthPlanet(GameState gameState)
        {
        }

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
            throw new NotImplementedException();
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
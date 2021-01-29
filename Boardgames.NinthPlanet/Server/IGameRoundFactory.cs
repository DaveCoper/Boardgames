using System.Collections.Generic;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Server
{
    public interface IGameRoundFactory
    {
        GameRound CreateGameRound(
            GameInfo gameInfo,
            SavedRoundState roundState);
    }
}
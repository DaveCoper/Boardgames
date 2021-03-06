﻿using Boardgames.NinthPlanet;
using Boardgames.Client.Models;

namespace Boardgames.WebServer.Repositories
{
    public interface INinthPlanetGameRepository : IGameRepository<Games.NinthPlanet, NinthPlanetNewGameOptions>
    {
    }
}
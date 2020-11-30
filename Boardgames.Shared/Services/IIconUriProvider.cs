using System;
using Boardgames.Shared.Models;

namespace Boardgames.Shared.Services
{
    public interface IIconUriProvider
    {
        Uri GetIconUri(GameType ninthPlanet, int resolution);
    }
}
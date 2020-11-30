using System;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public interface IIconUriProvider
    {
        Uri GetIconUri(GameType ninthPlanet, int resolution);
    }
}
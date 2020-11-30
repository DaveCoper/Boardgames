using System;
using Boardgames.Shared.Models;
using Boardgames.Shared.Services;

namespace Boardgames.Wpf.Client.Services
{
    public class IconUriProvider : IIconUriProvider
    {
        public Uri GetIconUri(GameType gameType, int resolution)
        {
            return new Uri($"pack://application:,,,/Boardgames.Wpf.Client;component/Resources/Images/{gameType}{resolution}x{resolution}.png");
        }
    }
}
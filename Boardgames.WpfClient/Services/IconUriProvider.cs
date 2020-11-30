using System;
using Boardgames.Client.Services;
using Boardgames.Common.Models;

namespace Boardgames.WpfClient.Services
{
    public class IconUriProvider : IIconUriProvider
    {
        public Uri GetIconUri(GameType gameType, int resolution)
        {
            return new Uri($"pack://application:,,,/Boardgames.WpfClient;component/Resources/Images/{gameType}{resolution}x{resolution}.png");
        }
    }
}
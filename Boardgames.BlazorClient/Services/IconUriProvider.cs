using System;
using Boardgames.Client.Services;
using Boardgames.Common.Models;

namespace Boardgames.BlazorClient.Services
{
    public class IconUriProvider : IIconUriProvider
    {
        private Uri baseUri;

        public IconUriProvider(Uri baseUri)
        {
            this.baseUri = baseUri;
        }

        public Uri GetIconUri(GameType gameType, int resolution)
        {
            return new Uri(baseUri, $"/Images/{gameType}{resolution}x{resolution}.png");
        }
    }
}
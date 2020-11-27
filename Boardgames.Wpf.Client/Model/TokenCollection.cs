using System;

namespace Boardgames.Wpf.Client.Model
{
    public class TokenCollection
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string IdentityToken { get; set; }
        public DateTime AccessTokenExpiration { get; internal set; }
    }
}
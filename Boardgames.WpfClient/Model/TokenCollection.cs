using System;

namespace Boardgames.WpfClient.Model
{
    public class TokenCollection
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string IdentityToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }
    }
}
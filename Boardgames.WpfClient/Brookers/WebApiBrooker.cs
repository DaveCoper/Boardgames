using Boardgames.Client.Brookers;
using Boardgames.WpfClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Boardgames.WpfClient.Brookers
{
    public class WebApiBrooker : IWebApiBrooker
    {
        private readonly HttpClient client;

        private readonly IAccessTokenProvider accessTokenProvider;

        public WebApiBrooker(HttpClient client, IAccessTokenProvider accessTokenProvider)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }

        public Uri BaseAddress { get; private set; } = new Uri("https://192.168.0.125:5001/");

        public async Task<TReturnType> GetAsync<TReturnType>(string controllerName, string actionName = null, IEnumerable<KeyValuePair<string, string>> parameters = null, CancellationToken cancellationToken = default)
        {
            Uri uri = CreateUri(controllerName, actionName, parameters);
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.GetFromJsonAsync<TReturnType>(uri, cancellationToken);
        }

        public async Task GetAsync(string controllerName, string actionName = null, IEnumerable<KeyValuePair<string, string>> parameters = null, CancellationToken cancellationToken = default)
        {
            Uri uri = CreateUri(controllerName, actionName, null);
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await client.GetAsync(uri, cancellationToken);
            result.EnsureSuccessStatusCode();
        }

        public async Task<TReturnType> PostAsync<TReturnType, TContentType>(
            string controllerName,
            TContentType content,
            string actionName = null,
            CancellationToken cancellationToken = default)
        {
            Uri uri = CreateUri(controllerName, actionName, null);
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var response = await client.PostAsJsonAsync(uri, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TReturnType>(resultJson);
        }

        public async Task PostAsync<TContentType>(
            string controllerName,
            TContentType content,
            string actionName = null,
            CancellationToken cancellationToken = default)
        {
            Uri uri = CreateUri(controllerName, actionName, null);
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var response = await client.PostAsJsonAsync(uri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private Uri CreateUri(string controllerName, string actionName, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Uri uri;
            if (string.IsNullOrEmpty(actionName))
            {
                uri = new Uri(this.BaseAddress, $"api/{controllerName}");
            }
            else
            {
                uri = new Uri(this.BaseAddress, $"api/{controllerName}/{actionName}");
            }

            if (parameters != null && parameters.Any())
            {
                var uriBuilder = new UriBuilder(uri);
                var queryBuilder = HttpUtility.ParseQueryString(string.Empty);

                foreach (var param in parameters)
                {
                    queryBuilder[param.Key] = param.Value;
                }

                uriBuilder.Query = queryBuilder.ToString();
                uri = uriBuilder.Uri;
            }

            return uri;
        }
    }
}
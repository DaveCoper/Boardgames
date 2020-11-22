using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using Boardgames.Web.Shared;
using IdentityModel.OidcClient;
using Newtonsoft.Json;

namespace Boardgames.Wpf.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var options = new OidcClientOptions()
            {
                //redirect to identity server
                Authority = "https://localhost:44399/",
                ClientId = "Boardgames.Wpf.Client",
                Scope = "openid profile offline_access Boardgames.Web.ServerAPI",
                //redirect back to app if auth success
                RedirectUri = "http://127.0.0.1:8080/boardgames/",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                Browser = new ExternalBrowser()
            };

            var client = new OidcClient(options);

            LoginResult loginResult;

            try
            {
                loginResult = await client.LoginAsync();
                this.Activate();
            }
            catch (Exception ex)
            {
                // activate even when login fails.
                this.Activate();
                MessageBox.Show(ex.Message, "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (loginResult.IsError)
            {
                MessageBox.Show(loginResult.Error, "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
                using (var response = await httpClient.GetAsync("https://localhost:44399/WeatherForecast"))
                {
                    string payload = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var forecast = JsonConvert.DeserializeObject<List<WeatherForecast>>(payload);
                        var formatedRows = forecast.Select(x => $"{x.Date}\t{x.TemperatureC}\t{x.Summary}").ToList();
                        payload = string.Join(Environment.NewLine, formatedRows);
                    }

                    if (string.IsNullOrWhiteSpace(payload))
                    {
                        payload = "Empty result";
                    }

                    MessageBox.Show(payload, response.StatusCode.ToString());
                }
            }
        }
    }
}
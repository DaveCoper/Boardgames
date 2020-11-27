﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Wpf.Client.Exceptions;
using Boardgames.Wpf.Client.Model;
using Boardgames.Wpf.Client.Services;
using GalaSoft.MvvmLight;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;

namespace Boardgames.Wpf.Client.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserDataStore userDataStore;

        private bool showLoginButton;

        private bool showLoginProgress;

        private TokenCollection tokens;

        [Obsolete("Required by WPF designer.", true)]
        public LoginViewModel()
        {
            if (!this.IsInDesignMode)
                throw new Exception();
        }

        public LoginViewModel(IUserDataStore userDataStore)
        {
            this.userDataStore = userDataStore ?? throw new ArgumentNullException(nameof(userDataStore));
        }

        public bool ShowLoginButton
        {
            get => showLoginButton;
            set => Set(ref showLoginButton, value);
        }

        public bool ShowLoginProgress
        {
            get => showLoginProgress;
            set => Set(ref showLoginProgress, value);
        }

        public TokenCollection Tokens
        {
            get => tokens;
            private set => Set(ref tokens, value);
        }

        public async Task<bool> RefreshTokenIsAvailableAsync()
        {
            var userData = await userDataStore.GetUserDataAsync();
            string refreshToken = userData.RefreshToken;
            return !string.IsNullOrWhiteSpace(refreshToken);
        }

        public async Task<TokenCollection> LoginAsync(CancellationToken cancellationToken)
        {
            SetLoginProgress(true);

            var userData = await userDataStore.GetUserDataAsync();
            string refreshToken = userData.RefreshToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                Tokens = await UseRefreshToken(refreshToken, cancellationToken);
            }
            else
            {
                Tokens = await UseFormLogin(cancellationToken);
            }

            userData.RefreshToken = Tokens.RefreshToken;
            await userDataStore.SaveUserDataAsync(userData);

            SetLoginProgress(false);
            return Tokens;
        }

        public async Task<TokenCollection> LoginUsingRefreshTokenAsync(CancellationToken cancellationToken)
        {
            SetLoginProgress(true);

            var userData = await userDataStore.GetUserDataAsync();
            string refreshToken = userData.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new LoginFailedException("Refresh token is not available!");
            }

            Tokens = await UseRefreshToken(refreshToken, cancellationToken);

            userData.RefreshToken = Tokens.RefreshToken;
            await userDataStore.SaveUserDataAsync(userData);

            SetLoginProgress(false);
            return Tokens;
        }

        private static OidcClient CreateOidcClient()
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
            return client;
        }

        private async Task<TokenCollection> UseFormLogin(CancellationToken cancellationToken)
        {
            OidcClient client = CreateOidcClient();
            LoginResult loginResult;
            try
            {
                loginResult = await client.LoginAsync(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                throw new LoginFailedException("Login failed", e);
            }

            if (loginResult.IsError)
            {
                throw new LoginFailedException(loginResult.Error);
            }

            return new TokenCollection
            {
                AccessTokenExpiration = loginResult.AccessTokenExpiration,
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                IdentityToken = loginResult.IdentityToken
            };
        }

        private async Task<TokenCollection> UseRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            var client = CreateOidcClient();

            RefreshTokenResult refreshResult;
            try
            {
                refreshResult = await client.RefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                throw new LoginFailedException("Login failed", e);
            }

            if (refreshResult.IsError)
            {
                throw new LoginFailedException(refreshResult.Error);
            }

            return new TokenCollection
            {
                AccessTokenExpiration = refreshResult.AccessTokenExpiration,
                AccessToken = refreshResult.AccessToken,
                RefreshToken = refreshResult.RefreshToken,
                IdentityToken = refreshResult.IdentityToken
            };
        }

        private void SetLoginProgress(bool loginInProgress)
        {
            ShowLoginButton = !loginInProgress;
            ShowLoginProgress = loginInProgress;
        }
    }
}
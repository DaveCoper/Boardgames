using System;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Wpf.Client.Model;
using Boardgames.Wpf.Client.ViewModels;

namespace Boardgames.Wpf.Client.Services
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly SemaphoreSlim semaphoreSlim;

        private readonly IDialogService dialogService;

        private readonly Func<LoginViewModel> loginViewModelFactory;

        private TokenCollection tokens = null;

        public AccessTokenProvider(IDialogService dialogService, Func<LoginViewModel> loginViewModelFactory)
        {
            this.semaphoreSlim = new SemaphoreSlim(1);
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.loginViewModelFactory = loginViewModelFactory ?? throw new ArgumentNullException(nameof(loginViewModelFactory));
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (tokens != null && tokens.AccessTokenExpiration < DateTime.Now - TimeSpan.FromSeconds(30))
            {
                return tokens.AccessToken;
            }

            await semaphoreSlim.WaitAsync();
            try
            {
                // try again to handle cases when 2 requests missed
                if (tokens != null && tokens.AccessTokenExpiration < DateTime.Now - TimeSpan.FromSeconds(30))
                {
                    return tokens.AccessToken;
                }

                var vm = loginViewModelFactory();
                if (await vm.RefreshTokenIsAvailableAsync())
                {
                    tokens = await vm.LoginUsingRefreshTokenAsync(cancellationToken);
                }
                else
                {
                    if(await dialogService.ShowDialogAsync(vm) != true)
                    {
                        throw new NotImplementedException();
                    }

                    tokens = vm.Tokens;
                }

                return tokens.AccessToken;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
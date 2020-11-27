using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Wpf.Client.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Boardgames.Wpf.Client.Services
{
    public class UserDataStore : IUserDataStore
    {
        private static readonly string UserDataFileName = "UserData.dat";

        private readonly SemaphoreSlim semaphoreSlim;

        private readonly ISecureStore secureStore;

        private readonly ILogger<UserDataStore> logger;

        private UserData? userData;

        public UserDataStore(ISecureStore secureStore) : this(secureStore, null)
        {
        }

        public UserDataStore(ISecureStore secureStore, ILogger<UserDataStore> logger)
        {
            this.semaphoreSlim = new SemaphoreSlim(1);

            this.secureStore = secureStore ?? throw new ArgumentNullException(nameof(secureStore));
            this.logger = logger ?? NullLogger<UserDataStore>.Instance;
        }

        public async Task<UserData> GetUserDataAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (userData == null)
                {
                    userData = await LoadUserData();
                }

                return userData.Value;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SaveUserDataAsync(UserData userData)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                this.userData = userData;
                await this.secureStore.WriteTextAsync(UserDataFileName, JsonConvert.SerializeObject(userData));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private static UserData CreateEmptyUserData()
        {
            return new UserData();
        }

        private async Task<UserData> LoadUserData()
        {
            string jsonData;
            try
            {
                jsonData = await secureStore.ReadAllTextAsync(UserDataFileName);
            }
            catch (FileNotFoundException e)
            {
                logger.LogInformation(e, "User settings file not found.");
                return CreateEmptyUserData();
            }

            try
            {
                return JsonConvert.DeserializeObject<UserData>(jsonData);
            }
            catch (JsonException e)
            {
                logger.LogError(e, "Failed to parse user data!");
                await secureStore.DeleteAsync(UserDataFileName);
                return CreateEmptyUserData();
            }
        }
    }
}
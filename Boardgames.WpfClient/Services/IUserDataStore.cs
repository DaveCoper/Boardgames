using System.Threading.Tasks;
using Boardgames.WpfClient.Model;

namespace Boardgames.WpfClient.Services
{
    public interface IUserDataStore
    {
        public Task<UserData> GetUserDataAsync();

        public Task SaveUserDataAsync(UserData userData);
    }
}
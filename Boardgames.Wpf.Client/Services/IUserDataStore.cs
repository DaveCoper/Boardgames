using System.Threading.Tasks;
using Boardgames.Wpf.Client.Model;

namespace Boardgames.Wpf.Client.Services
{
    public interface IUserDataStore
    {
        public Task<UserData> GetUserDataAsync();

        public Task SaveUserDataAsync(UserData userData);
    }
}
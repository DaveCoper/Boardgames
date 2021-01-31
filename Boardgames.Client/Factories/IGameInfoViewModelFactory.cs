using Boardgames.Client.ViewModels;
using Boardgames.Common.Models;

namespace Boardgames.Client.Factories
{
    public interface IGameInfoViewModelFactory
    {
        GameInfoViewModel CreateInstance(GameInfo gameInfo, PlayerData gameOwner);
    }
}
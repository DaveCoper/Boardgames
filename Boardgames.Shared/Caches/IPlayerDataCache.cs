using Boardgames.Shared.Models;

namespace Boardgames.Shared.Caches
{
    public interface IPlayerDataCache
    {
        bool TryGetValue(int playerId, out PlayerData playerData);

        bool TryAddValue(int playerId, PlayerData playerData);
    }
}
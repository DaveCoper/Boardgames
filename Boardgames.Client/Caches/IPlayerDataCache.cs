using Boardgames.Common.Models;

namespace Boardgames.Client.Caches
{
    public interface IPlayerDataCache
    {
        bool TryGetValue(int playerId, out PlayerData playerData);

        bool TryAddValue(int playerId, PlayerData playerData);
    }
}
namespace Boardgames.Web.Server.Repositories
{
    public interface IGameCache<IGameType>
    {
        bool TryAddGame(int gameId, IGameType game);

        bool TryGetGame(int gameId, out IGameType game);
    }
}